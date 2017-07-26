using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Controls;
using ICD.Connect.Displays;
using ICD.Connect.Rooms;
using ICD.Connect.Rooms.Extensions;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.Routing.Extensions;
using ICD.MetLife.RoomOS.Endpoints.Destinations;
using ICD.MetLife.RoomOS.Endpoints.Sources;

namespace ICD.MetLife.RoomOS.Rooms
{
	public sealed class MetlifeRouting : IDisposable
	{
		/// <summary>
		/// Raised when a detected source is automatically routed to the current destination.
		/// </summary>
		public event EventHandler<SourceEventArgs> OnSourceAutoRouted;

		/// <summary>
		/// Raised when a source device starts/stops sending video.
		/// </summary>
		public event EventHandler OnSourceTransmissionStateChanged;

		/// <summary>
		/// Raised when a source device is connected or disconnected.
		/// </summary>
		public event EventHandler OnSourceDetectionStateChanged;

		/// <summary>
		/// Called when a switcher changes routing.
		/// </summary>
		public event EventHandler OnRouteChanged;

		private readonly MetlifeRoom m_MetlifeRoom;
		private readonly IcdHashSet<IRouteSourceControl> m_SubscribedSourceControls;
		private readonly IcdHashSet<IRouteDestinationControl> m_SubscribedDestinationControls;
		private readonly IcdHashSet<IRouteSwitcherControl> m_SubscribedSwitchers;

		/// <summary>
		/// Tracks the current user overrides for the destinations.
		/// </summary>
		private readonly IcdHashSet<MetlifeDestination> m_DestinationOverrides;

		private readonly SafeCriticalSection m_DestinationOverridesSection;

		private bool m_RouteThroughCodec;

		#region Properties

		private ILoggerService Logger
		{
			get { return ServiceProvider.TryGetService<ILoggerService>(); }
		}

		private IRoutingGraph RoutingGraph { get { return m_MetlifeRoom.Core.GetRoutingGraph(); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		public MetlifeRouting(MetlifeRoom parent)
		{
			m_SubscribedSourceControls = new IcdHashSet<IRouteSourceControl>();
			m_SubscribedDestinationControls = new IcdHashSet<IRouteDestinationControl>();
			m_SubscribedSwitchers = new IcdHashSet<IRouteSwitcherControl>();
			m_DestinationOverrides = new IcdHashSet<MetlifeDestination>();
			m_DestinationOverridesSection = new SafeCriticalSection();

			m_MetlifeRoom = parent;

			Subscribe(m_MetlifeRoom);
			SubscribeSourceControls();
			SubscribeDestinationControls();
			SubscribeSwitchers();
		}

		#region Methods

		/// <summary>
		/// Release Resources
		/// </summary>
		public void Dispose()
		{
			OnSourceAutoRouted = null;
			OnSourceTransmissionStateChanged = null;
			OnSourceDetectionStateChanged = null;
			OnRouteChanged = null;

			Unsubscribe(m_MetlifeRoom);
			UnsubscribeSourceControls();
			UnsubscribeDestinationControls();
			UnsubscribeSwitchers();
		}

		#region Sources

		/// <summary>
		/// Gets the source matching the given control, output and connection type.
		/// </summary>
		/// <param name="sourceControl"></param>
		/// <param name="output"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		[CanBeNull]
		public MetlifeSource GetSource(IRouteSourceControl sourceControl, int output, eConnectionType type)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			return GetSource(sourceControl.GetOutputEndpointInfo(output), type);
		}

		/// <summary>
		/// Gets the source matching the given endpoint and connection type.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		[CanBeNull]
		public MetlifeSource GetSource(EndpointInfo endpoint, eConnectionType type)
		{
			return GetSources().FirstOrDefault(s => s.Endpoint == endpoint && EnumUtils.HasFlags(s.ConnectionType, type));
		}

		/// <summary>
		/// Gets the metlife sources in order.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MetlifeSource> GetSources()
		{
			return m_MetlifeRoom.Sources
			                    .Where(d => !d.Disable)
			                    .OfType<MetlifeSource>()
			                    .OrderBy(s => s.Order)
			                    .ThenBy(s => s.GetNameOrDeviceName(m_MetlifeRoom));
		}

		/// <summary>
		/// Gets the sources that match the given flags.
		/// </summary>
		/// <param name="sourceFlags"></param>
		/// <returns></returns>
		public IEnumerable<MetlifeSource> GetSources(eSourceFlags sourceFlags)
		{
			return GetSources().Where(s => s.SourceFlags.HasFlag(sourceFlags));
		}

		/// <summary>
		/// Gets the sources that are available in the share menu.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MetlifeSource> GetOnlineSources()
		{
			return GetSources().Where(s => !s.Disable && m_MetlifeRoom.Devices[s.Endpoint.Device].IsOnline);
		}

		/// <summary>
		/// Gets the source currently routed to the given destination.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="type"></param>
		/// <param name="signalDetected"></param>
		/// <returns></returns>
		[CanBeNull]
		public MetlifeSource GetActiveSource(IDestination destination, eConnectionType type, bool signalDetected)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			return GetActiveSource(destination.Endpoint, type, signalDetected);
		}

		/// <summary>
		/// Gets the source currently routed to the given destination.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="type"></param>
		/// <param name="signalDetected"></param>
		/// <returns></returns>
		[CanBeNull]
		public MetlifeSource GetActiveSource(EndpointInfo destination, eConnectionType type, bool signalDetected)
		{
			IRouteDestinationControl destinationControl = RoutingGraph.GetDestinationControl(destination);

			EndpointInfo source;
			bool found = RoutingGraph.GetActiveSourceEndpoints(destinationControl, destination.Address, type, signalDetected)
									 .TryFirstOrDefault(out source);

			return found ? m_MetlifeRoom.Sources.OfType<MetlifeSource>().FirstOrDefault(s => s.Endpoint == source) : null;
		}

		/// <summary>
		/// Gets the sources that are actively routed to any display.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MetlifeSource> GetActiveSources()
		{
			return GetDisplayDestinations().Select(d => GetActiveSource(d, eConnectionType.Video, true))
			                               .Where(s => s != null);
		}

		/// <summary>
		/// Simple check to see if the source is transmitting and detected by the next node in the routing graph.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool SourceDetected(MetlifeSource source, eConnectionType type)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IRouteSourceControl sourceControl = RoutingGraph.GetSourceControl(source);
			if (sourceControl == null)
				return false;

			bool detected = RoutingGraph.SourceDetected(sourceControl, source.Endpoint.Address, eConnectionType.Video);
			if (source.EnableWhenNotTransmitting)
				return detected;

			bool activeTransmission = sourceControl.GetActiveTransmissionState(source.Endpoint.Address, type);
			return detected && activeTransmission;
		}

		#endregion

		#region Destinations

		/// <summary>
		/// Gets the destination matching the given endpoint info.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		[CanBeNull]
		public MetlifeDestination GetDestination(EndpointInfo info)
		{
			return GetDestinations().FirstOrDefault(d => d.Endpoint == info);
		}

		/// <summary>
		/// Gets the ordered MetlifeDestinations.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MetlifeDestination> GetDestinations()
		{
			return m_MetlifeRoom.Destinations
			                    .Where(d => !d.Disable)
			                    .OfType<MetlifeDestination>()
			                    .OrderBy(s => s.Order)
			                    .ThenBy(s => s.GetNameOrDeviceName(m_MetlifeRoom));
		}

		/// <summary>
		/// Gets the destinations that are displays.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MetlifeDestination> GetDisplayDestinations()
		{
			return GetDestinations().Where(d => m_MetlifeRoom.Devices[d.Endpoint.Device] is IDisplay);
		}

		/// <summary>
		/// Clears the user overrides and reroutes everything.
		/// </summary>
		public void ClearOverrides()
		{
			ClearOverrides(true);
		}

		/// <summary>
		/// Clears the user overrides. Reroutes everything if updateRouting is true.
		/// </summary>
		/// <param name="updateRouting"></param>
		private void ClearOverrides(bool updateRouting)
		{
			m_DestinationOverridesSection.Execute(() => m_DestinationOverrides.Clear());

			if (updateRouting)
				UpdateRouteThroughCodec();
		}

		/// <summary>
		/// Returns true if the given destination is being overridden.
		/// </summary>
		/// <returns></returns>
		public bool IsOverride(MetlifeDestination destination)
		{
			return m_DestinationOverridesSection.Execute(() => m_DestinationOverrides.Contains(destination));
		}

		/// <summary>
		/// Gets the destinations that have user configured overrides.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MetlifeDestination> GetOverrides()
		{
			return m_DestinationOverridesSection.Execute(() => m_DestinationOverrides.ToArray());
		}

		/// <summary>
		/// Gets the destinations the source is currently routed to.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="type"></param>
		/// <param name="active"></param>
		/// <returns></returns>
		public IEnumerable<MetlifeDestination> GetActiveDestinations(ISource source, eConnectionType type, bool active)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IRouteSourceControl sourceControl = RoutingGraph.GetSourceControl(source);

			return RoutingGraph.GetActiveDestinationEndpoints(sourceControl, source.Endpoint.Address, type, active)
			                    .Select(e => GetDestination(e))
			                    .Where(d => d != null);
		}

		#endregion

		#region Codec

		/// <summary>
		/// When true we route the codec to the displays, and sources are routed through the codec to the displays.
		/// When false sources are routed directly to the displays.
		/// </summary>
		public void SetRouteThroughCodec(bool value)
		{
			if (value == m_RouteThroughCodec)
				return;

			m_RouteThroughCodec = value;

			if (m_RouteThroughCodec)
				RouteThroughCodec();
			else
				UnrouteThroughCodec();
		}

		/// <summary>
		/// Simply returns the input address being used with the codec.
		/// </summary>
		/// <returns></returns>
		public int GetCodecInput()
		{
			CiscoCodecRoutingControl destinationControl = GetCodecRoutingControl();
			if (destinationControl == null)
				throw new InvalidOperationException("No codec routing control.");

			return RoutingGraph.Connections.GetInputs(destinationControl, eConnectionType.Video).First();
		}

		/// <summary>
		/// Gets the endpoint info for the codec source input.
		/// </summary>
		/// <returns></returns>
		public EndpointInfo GetCodecInputEndpointInfo()
		{
			CiscoCodecRoutingControl control = GetCodecRoutingControl();
			int input = GetCodecInput();

			return control.GetInputEndpointInfo(input);
		}

		/// <summary>
		/// Gets the source currently routed to the codec.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		public MetlifeSource GetCodecSource()
		{
			IRouteDestinationControl codecControl = GetCodecRoutingControl();
			if (codecControl == null)
				return null;

			int input = GetCodecInput();

			EndpointInfo endpoint;
			bool found = RoutingGraph.GetActiveSourceEndpoints(codecControl, input, eConnectionType.Video, false)
			                         .TryFirstOrDefault(out endpoint);

			return found ? GetSource(endpoint, eConnectionType.Video) : null;
		}

		/// <summary>
		/// Gets the routing control for the codec.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private CiscoCodecRoutingControl GetCodecRoutingControl()
		{
			CiscoCodec codec = m_MetlifeRoom.GetDevice<CiscoCodec>();
			return codec == null ? null : codec.Controls.GetControl<CiscoCodecRoutingControl>();
		}

		#endregion

		#region Routing

		/// <summary>
		/// Returns true if the source is currently routed to the codec.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public bool IsRoutedToCodec(MetlifeSource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return IsRoutedToCodec(source.Endpoint);
		}

		/// <summary>
		/// Returns true if the source is currently routed to the codec.
		/// </summary>
		/// <param name="sourceControl"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		private bool IsRoutedToCodec(IRouteSourceControl sourceControl, int output)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			return IsRoutedToCodec(sourceControl.GetOutputEndpointInfo(output));
		}

		/// <summary>
		/// Returns true if the source is currently routed to the codec.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private bool IsRoutedToCodec(EndpointInfo source)
		{
			CiscoCodec codec = m_MetlifeRoom.GetDevice<CiscoCodec>();
			if (codec == null)
				return false;

			EndpointInfo codecInput = GetCodecInputEndpointInfo();
			return IsRoutedToDestination(source, codecInput, eConnectionType.Video, true);
		}

		/// <summary>
		/// Returns true if the source is routed to the destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <param name="type"></param>
		/// <param name="detected"></param>
		/// <returns></returns>
		public bool IsRoutedToDestination(MetlifeSource source, MetlifeDestination destination, eConnectionType type, bool detected)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			return IsRoutedToDestination(source.Endpoint, destination.Endpoint, type, detected);
		}

		/// <summary>
		/// Returns true if the source is routed to the destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <param name="type"></param>
		/// <param name="detected"></param>
		/// <returns></returns>
		public bool IsRoutedToDestination(EndpointInfo source, EndpointInfo destination, eConnectionType type, bool detected)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			return RoutingGraph.GetActiveSourceEndpoints(destination, type, detected)
			                   .AnyAndAll(e => e == source);
		}

		/// <summary>
		/// When not in a call
		///		Routes the source to all displays that are enabled and have “share by default” as true
		///		Routes the source to the program audio output.
		/// While in a video call
		///		Route the source to the content input of the codec and shares it
		///		Routes the audio to the rooms call audio output - only if call audio is different to program audio.
		/// </summary>
		/// <param name="source"></param>
		public void Route(MetlifeSource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			RouteAudio(source);

			bool videoCall = m_MetlifeRoom.ConferenceManager.IsInVideoCall;

			if (videoCall)
			{
				RouteVideoToCodec(source);
				return;
			}

			IEnumerable<MetlifeDestination> videoDestinations =
				GetDestinations().Where(d => d.ShareByDefault && d.ConnectionType.HasFlag(eConnectionType.Video));

			foreach (MetlifeDestination videoDestination in videoDestinations)
				RouteVideoToDestination(source, videoDestination);
		}

		/// <summary>
		/// Routes the given codec output to the destination.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="destination"></param>
		private void RouteCodecToDestination(int output, MetlifeDestination destination)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			EndpointInfo source = GetCodecRoutingControl().GetOutputEndpointInfo(output);
			RouteVideoToDestination(source, destination.Endpoint);
		}

		/// <summary>
		/// Routes the source video to the destination and powers the display.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		private void RouteVideoToDestination(MetlifeSource source, MetlifeDestination destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			RouteVideoToDestination(source.Endpoint, destination.Endpoint);
		}

		private void UnrouteVideoFromDestination(MetlifeSource source, MetlifeDestination destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			UnrouteVideoFromDestination(source.Endpoint, destination.Endpoint);
		}

		/// <summary>
		/// Routes the source video to the destination and powers the display.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		private void RouteVideoToDestination(EndpointInfo source, EndpointInfo destination)
		{
			RoutingGraph.Route(source, destination, eConnectionType.Video, m_MetlifeRoom.Id);

			IDisplay display = m_MetlifeRoom.Devices.GetInstance<IDisplay>(destination.Device);

			display.PowerOn();
			display.SetHdmiInput(destination.Address);
		}

		/// <summary>
		/// Unroutes the source video from the destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		private void UnrouteVideoFromDestination(EndpointInfo source, EndpointInfo destination)
		{
			RoutingGraph.Unroute(source, destination, eConnectionType.Video, m_MetlifeRoom.Id);
		}

		/// <summary>
		/// Routes audio for the given source to the current audio destination.
		/// Skips routing if in a call and the call and program outputs are the same.
		/// </summary>
		/// <param name="source"></param>
		public void RouteAudio(MetlifeSource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IEnumerable<EndpointInfo> audioDestinations = GetAudioDestinationsForRouting();

			foreach (EndpointInfo audioDestination in audioDestinations)
				RoutingGraph.Route(source.Endpoint, audioDestination, eConnectionType.Audio, m_MetlifeRoom.Id);
		}

		/// <summary>
		/// Returns valid audio destinations for routing based on the current in-call status.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<EndpointInfo> GetAudioDestinationsForRouting()
		{
			IEnumerable<EndpointInfo> callDestinations =
				GetDestinations().Where(d => d.AudioOption.HasFlag(MetlifeDestination.eAudioOption.Call))
				                 .Select(d => d.Endpoint);

			IEnumerable<EndpointInfo> programDestinations =
				GetDestinations().Where(d => d.AudioOption.HasFlag(MetlifeDestination.eAudioOption.Program))
								 .Select(d => d.Endpoint);

			return m_MetlifeRoom.ConferenceManager.IsInVideoCall
				? callDestinations.Except(programDestinations)
				: programDestinations;
		}

		/// <summary>
		/// Routes video from the source to the destination.
		/// Sets the override state for the destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public void RouteVideoToDestinationOverride(MetlifeSource source, MetlifeDestination destination)
		{
			m_DestinationOverridesSection.Execute(() => m_DestinationOverrides.Add(destination));
			RouteVideoToDestination(source, destination);
		}

		/// <summary>
		/// Unroutes the source video from the destination.
		/// Clears the override state for the destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public void UnrouteVideoFromDestinationOverride(MetlifeSource source, MetlifeDestination destination)
		{
			m_DestinationOverridesSection.Execute(() => m_DestinationOverrides.Remove(destination));
			UnrouteVideoFromDestination(source, destination);
		}

		/// <summary>
		/// Routes the video from the source to the codec input.
		/// </summary>
		/// <param name="source"></param>
		public void RouteVideoToCodec(MetlifeSource source)
		{
			RoutingGraph.Route(source.Endpoint, GetCodecInputEndpointInfo(), eConnectionType.Video, m_MetlifeRoom.Id);
			m_MetlifeRoom.StartPresentation();
		}

		/// <summary>
		/// Unroutes the source video from the codec input.
		/// </summary>
		/// <param name="source"></param>
		public void UnrouteVideoFromCodec(MetlifeSource source)
		{
			RoutingGraph.Unroute(source.Endpoint, GetCodecInputEndpointInfo(), eConnectionType.Video, m_MetlifeRoom.Id);
			if (GetCodecSource() == null)
				m_MetlifeRoom.StopPresentation();
		}

		/// <summary>
		/// Unroutes all of the sources.
		/// </summary>
		public void UnrouteSources()
		{
			GetSources().ForEach(UnrouteSource);
		}

		/// <summary>
		/// Unroutes the given source.
		/// </summary>
		/// <param name="source"></param>
		public void UnrouteSource(MetlifeSource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IRouteSourceControl sourceControl = RoutingGraph.GetSourceControl(source);
			UnrouteSource(sourceControl, source.Endpoint.Address, source.ConnectionType);
		}

		/// <summary>
		/// Completely unroutes the given source.
		/// </summary>
		/// <param name="sourceControl"></param>
		/// <param name="output"></param>
		private void UnrouteSource(IRouteSourceControl sourceControl, int output)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			UnrouteSource(sourceControl, output, EnumUtils.GetFlagsAllValue<eConnectionType>());
		}

		/// <summary>
		/// Completely unroutes the given source.
		/// </summary>
		/// <param name="sourceControl"></param>
		/// <param name="output"></param>
		/// <param name="type"></param>
		private void UnrouteSource(IRouteSourceControl sourceControl, int output, eConnectionType type)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			// Prevents the codec from being unrouted from the displays
			if (type.HasFlag(eConnectionType.Video) && IsRoutedToCodec(sourceControl, output))
				RoutingGraph.Unroute(sourceControl, GetCodecRoutingControl(), type, m_MetlifeRoom.Id);
			
			RoutingGraph.Unroute(sourceControl, output, type, m_MetlifeRoom.Id);

			// End the presentation if nothing is routed to the codec.
			if (GetCodecSource() == null)
				m_MetlifeRoom.StopPresentation();

			// Update the inactivity timer since there may be nothing routed.
			m_MetlifeRoom.ResetInactivityTimer();
		}

		#endregion

		#endregion

		#region Private Methods

		/// <summary>
		/// Auto-routes source controls that become connected/disconnected.
		/// </summary>
		/// <param name="sourceControl"></param>
		/// <param name="output"></param>
		/// <param name="type"></param>
		private void HandleAutoRouting(IRouteSourceControl sourceControl, int output, eConnectionType type)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			// Only auto-route Share sources
			MetlifeSource source = GetSource(sourceControl, output, type);
			if (source == null || !source.SourceFlags.HasFlag(eSourceFlags.Share))
				return;

			bool detected = RoutingGraph.SourceDetected(sourceControl, output, type) &&
			                sourceControl.GetActiveTransmissionState(output, type);

			// If a source started transmitting route it immediately, otherwise unroute it.
			if (detected && !source.InhibitAutoRoute)
				AutoRouteDetectedSource(sourceControl, output);
			else if (!detected && !source.InhibitAutoUnroute)
				AutoUnrouteUndetectedSource(sourceControl, output);
		}

		/// <summary>
		/// Routes the detected source to the current destination IF there is currently no routed source.
		/// Starts a presentation if the destination is the codec.
		/// </summary>
		/// <param name="sourceControl"></param>
		/// <param name="output"></param>
		private void AutoRouteDetectedSource(IRouteSourceControl sourceControl, int output)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			if (GetActiveSources().Any())
				return;

			EndpointInfo info = sourceControl.GetOutputEndpointInfo(output);
			MetlifeSource source = GetSource(info, eConnectionType.Video);
			if (source == null)
				return;

			Route(source);

			OnSourceAutoRouted.Raise(this, new SourceEventArgs(source));
		}

		/// <summary>
		/// Unroutes the undetected source.
		/// Stops the presentation if the source was the active source for the destination.
		/// </summary>
		/// <param name="sourceControl"></param>
		/// <param name="output"></param>
		private void AutoUnrouteUndetectedSource(IRouteSourceControl sourceControl, int output)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			UnrouteSource(sourceControl, output);
		}

		/// <summary>
		/// Re-routes everything based on the current route through codec mode.
		/// </summary>
		private void UpdateRouteThroughCodec()
		{
			if (m_RouteThroughCodec)
				RouteThroughCodec();
			else
				UnrouteThroughCodec();
		}

		/// <summary>
		/// Route every display according to “VtcOption” parameter
		///		Main - gets codec output 1
		///		Secondary - gets codec output 2
		///		ContentOnly - depends on when content is being shared by either local or far end
		///			if no content sharing, unroute display (but leave on).  When either side is sharing, show codec output 2
		///		None - unroute display
		/// </summary>
		private void RouteThroughCodec()
		{
			MetlifeSource active = GetActiveSources().FirstOrDefault();

			// Clear everything
			ClearOverrides(false);
			UnrouteSources();

			// Automatically route the source that was previously active.
			if (active != null)
				RouteVideoToCodec(active);

			// Route codec to the displays
			RouteCodecToDisplays();
		}

		/// <summary>
		/// Route every display according to “VtcOption” parameter
		///		Main - gets codec output 1
		///		Secondary - gets codec output 2
		///		ContentOnly - depends on when content is being shared by either local or far end
		///			if no content sharing, unroute display (but leave on).  When either side is sharing, show codec output 2
		///		None - unroute display
		/// </summary>
		private void RouteCodecToDisplays()
		{
			MetlifeSource active = GetActiveSources().FirstOrDefault();

			foreach (MetlifeDestination destination in GetDisplayDestinations())
			{
				switch (destination.VtcOption)
				{
					case MetlifeDestination.eVtcOption.None:
						break;

					case MetlifeDestination.eVtcOption.Main:
						RouteCodecToDestination(1, destination);
						break;
					case MetlifeDestination.eVtcOption.Secondary:
						RouteCodecToDestination(2, destination);
						break;

					case MetlifeDestination.eVtcOption.ContentOnly:
						if (active != null)
							RouteCodecToDestination(2, destination);
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// If VTC Content Source != null when call ended
		///		Route content to all displays that are
		///			Enabled
		///			ShareByDefault
		///			NOT Override
		///		Route content to room audio output
		///	If VTC Content Source == null
		///		Clear content to all displays that are
		///			Enabled
		///			ShareByDefault
		///			NOT Override
		///	Clear all override flags
		/// </summary>
		private void UnrouteThroughCodec()
		{
			MetlifeSource active = GetActiveSources().FirstOrDefault();

			// Clear everything
			ClearOverrides(false);
			UnrouteSources();

			if (active == null)
				return;

			foreach (MetlifeDestination destination in GetDisplayDestinations().Where(d => d.ShareByDefault && !IsOverride(d)))
				RouteVideoToDestination(active, destination);
		}

		#endregion

		#region MetlifeRoom Callbacks

		/// <summary>
		/// Subscribe to the metlife room events.
		/// </summary>
		/// <param name="metlifeRoom"></param>
		private void Subscribe(MetlifeRoom metlifeRoom)
		{
			if (metlifeRoom == null)
				throw new ArgumentNullException("metlifeRoom");

			metlifeRoom.Devices.OnChildrenChanged += MetlifeRoomOnDevicesChanged;
		}

		/// <summary>
		/// Subscribe to the metlife room events.
		/// </summary>
		/// <param name="metlifeRoom"></param>
		private void Unsubscribe(MetlifeRoom metlifeRoom)
		{
			if (metlifeRoom == null)
				throw new ArgumentNullException("metlifeRoom");

			metlifeRoom.Devices.OnChildrenChanged -= MetlifeRoomOnDevicesChanged;
		}

		/// <summary>
		/// Called when a route operation completes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void RoutingGraphOnRouteFinished(object sender, RouteFinishedEventArgs args)
		{
			if (args.Success)
				Logger.AddEntry(eSeverity.Debug, "Successfully routed {0}", args.Route.ToStringLocal());
			else
				Logger.AddEntry(eSeverity.Error, "Failed to route {0}", args.Route.ToStringLocal());
		}

		/// <summary>
		/// Called when the devices in the room change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void MetlifeRoomOnDevicesChanged(object sender, EventArgs eventArgs)
		{
			SubscribeSourceControls();
			SubscribeDestinationControls();
			SubscribeSwitchers();
		}

		#endregion

		#region Destination Callbacks

		/// <summary>
		/// Unsubscribe from the previous destination controls and subscribe to the new destination control events.
		/// </summary>
		private void SubscribeDestinationControls()
		{
			UnsubscribeDestinationControls();

			foreach (IRouteDestinationControl destination in m_MetlifeRoom.GetControls<IRouteDestinationControl>())
			{
				Subscribe(destination);
				m_SubscribedDestinationControls.Add(destination);
			}
		}

		/// <summary>
		/// Unsubscribe from the previous destination control events.
		/// </summary>
		private void UnsubscribeDestinationControls()
		{
			foreach (IRouteDestinationControl destinationControl in m_SubscribedDestinationControls)
				Unsubscribe(destinationControl);
			m_SubscribedDestinationControls.Clear();
		}

		/// <summary>
		/// Subscribe to the destination control events.
		/// </summary>
		/// <param name="destinationControl"></param>
		private void Subscribe(IRouteDestinationControl destinationControl)
		{
			if (destinationControl == null)
				throw new ArgumentNullException("destinationControl");

			destinationControl.OnSourceDetectionStateChange += DestinationControlOnSourceDetectionStateChange;
			destinationControl.OnActiveInputsChanged += DestinationControlOnActiveInputsChanged;
		}

		/// <summary>
		/// Unsubscribe from the destination control events.
		/// </summary>
		/// <param name="destinationControl"></param>
		private void Unsubscribe(IRouteDestinationControl destinationControl)
		{
			if (destinationControl == null)
				throw new ArgumentNullException("destinationControl");

			destinationControl.OnSourceDetectionStateChange -= DestinationControlOnSourceDetectionStateChange;
			destinationControl.OnActiveInputsChanged -= DestinationControlOnActiveInputsChanged;
		}

		/// <summary>
		/// Called when a destination control source detection state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DestinationControlOnSourceDetectionStateChange(object sender, SourceDetectionStateChangeEventArgs args)
		{
			if (args == null)
				throw new ArgumentNullException("args");

			int output;

			IRouteDestinationControl destination = sender as IRouteDestinationControl;

			// Get the immediate source connected to the destination input address.
			IRouteSourceControl source =
				RoutingGraph.GetSourceControl(destination, args.Input, args.Type, out output);

			if (source != null)
				HandleAutoRouting(source, output, args.Type);

			OnSourceDetectionStateChanged.Raise(this);
		}

		/// <summary>
		/// Called when a destination control changes the inputs it is using.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void DestinationControlOnActiveInputsChanged(object sender, EventArgs eventArgs)
		{
			// I think this was a workaround for displays changing HDMI input to raise a route change event.
			OnRouteChanged.Raise(this);
		}

		#endregion

		#region Source Callbacks

		/// <summary>
		/// Unsubscribe from the previous source controls and subscribe to the current source events.
		/// </summary>
		private void SubscribeSourceControls()
		{
			UnsubscribeSourceControls();

			foreach (IRouteSourceControl control in m_MetlifeRoom.GetControls<IRouteSourceControl>())
			{
				Subscribe(control);
				m_SubscribedSourceControls.Add(control);
			}
		}

		/// <summary>
		/// Unsubscribe from the previous source control events.
		/// </summary>
		private void UnsubscribeSourceControls()
		{
			foreach (IRouteSourceControl control in m_SubscribedSourceControls)
				Unsubscribe(control);
			m_SubscribedSourceControls.Clear();
		}

		/// <summary>
		/// Subscribe to the source control events.
		/// </summary>
		/// <param name="sourceControl"></param>
		private void Subscribe(IRouteSourceControl sourceControl)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			sourceControl.OnActiveTransmissionStateChanged += SourceControlOnActiveTransmissionStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the source control events.
		/// </summary>
		/// <param name="sourceControl"></param>
		private void Unsubscribe(IRouteSourceControl sourceControl)
		{
			if (sourceControl == null)
				throw new ArgumentNullException("sourceControl");

			sourceControl.OnActiveTransmissionStateChanged -= SourceControlOnActiveTransmissionStateChanged;
		}

		/// <summary>
		/// Called when a source control starts/stops sending video.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SourceControlOnActiveTransmissionStateChanged(object sender, TransmissionStateEventArgs args)
		{
			IRouteSourceControl source = sender as IRouteSourceControl;
			if (source == null)
				return;

			if (args == null)
				throw new ArgumentNullException("args");

			HandleAutoRouting(source, args.Output, args.Type);

			OnSourceTransmissionStateChanged.Raise(this);
		}

		#endregion

		#region Switcher Callbacks

		/// <summary>
		/// Unsubscribe from the previous switchers and subscribe to the current switcher events.
		/// </summary>
		private void SubscribeSwitchers()
		{
			UnsubscribeSwitchers();

			foreach (IRouteSwitcherControl control in m_MetlifeRoom.GetControls<IRouteSwitcherControl>())
			{
				Subscribe(control);
				m_SubscribedSwitchers.Add(control);
			}
		}

		/// <summary>
		/// Unsubscribe from the previous switcher events.
		/// </summary>
		private void UnsubscribeSwitchers()
		{
			foreach (IRouteSwitcherControl control in m_SubscribedSwitchers)
				Unsubscribe(control);
			m_SubscribedSwitchers.Clear();
		}

		/// <summary>
		/// Subscribe to the switcher events.
		/// </summary>
		/// <param name="switcher"></param>
		private void Subscribe(IRouteSwitcherControl switcher)
		{
			if (switcher == null)
				throw new ArgumentNullException("switcher");

			switcher.OnRouteChange += SwitcherOnRouteChange;
		}

		/// <summary>
		/// Unsubscribe from the switcher events.
		/// </summary>
		/// <param name="switcher"></param>
		private void Unsubscribe(IRouteSwitcherControl switcher)
		{
			if (switcher == null)
				throw new ArgumentNullException("switcher");

			switcher.OnRouteChange -= SwitcherOnRouteChange;
		}

		/// <summary>
		/// Called when a switcher changes route.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SwitcherOnRouteChange(object sender, RouteChangeEventArgs args)
		{
			OnRouteChanged.Raise(this);
		}

		#endregion
	}
}
