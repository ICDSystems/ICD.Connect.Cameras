using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Nodes;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Components.Directory.Tree;
using ICD.Connect.Conferencing.Cisco.Components.Presentation;
using ICD.Connect.Conferencing.Cisco.Components.Video;
using ICD.Connect.Conferencing.Cisco.Components.Video.Connectors;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.Controls;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Favorites.SqLite;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Displays;
using ICD.Connect.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Settings.Core;
using ICD.Connect.TvPresets;
using ICD.MetLife.RoomOS.Endpoints.Destinations;
using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.VolumePoints;

namespace ICD.MetLife.RoomOS.Rooms
{
	/// <summary>
	/// Contains the devices and functionality for a Metlife Room.
	/// </summary>
	public sealed class MetlifeRoom : AbstractRoom<MetlifeRoomSettings>
	{
		private const long SHUTDOWN_TIME = 60 * 1000;

		public event EventHandler<BoolEventArgs> OnOccupiedStateChanged;

		/// <summary>
		/// Raised when the room shuts down.
		/// </summary>
		[PublicAPI]
		public event EventHandler OnShutdown;

		private readonly SafeTimer m_InactivityTimer;
		private readonly IConferenceManager m_ConferenceManager;
		private readonly MetlifeRoomOwner m_Owner;
		private readonly IcdTimer m_ShutdownTimer;
		private readonly MetlifeRouting m_Routing;
		private readonly List<VolumePoint> m_VolumePoints;

		private bool m_IsOccupied;

		// Used with settings.
		private string m_TvPresetsPath;
		private int m_InactivitySeconds;

		#region Properties

		/// <summary>
		/// Gets the name prefix for the room.
		/// </summary>
		public string Prefix { get; private set; }

		/// <summary>
		/// The room configuration name.
		/// </summary>
		[PublicAPI]
		public string Config { get; private set; }

		/// <summary>
		/// Gets the room number.
		/// </summary>
		public string Number { get; private set; }

		/// <summary>
		/// Gets the room telephone number.
		/// </summary>
		public string PhoneNumber { get; private set; }

		/// <summary>
		/// Gets the room owner information.
		/// </summary>
		[PublicAPI]
		public MetlifeRoomOwner Owner { get { return m_Owner; } }

		/// <summary>
		/// Gets the shutdown timer.
		/// </summary>
		[PublicAPI]
		public IcdTimer ShutdownTimer { get { return m_ShutdownTimer; } }

		/// <summary>
		/// Gets the tv presets.
		/// </summary>
		public ITvPresets TvPresets { get; private set; }

		/// <summary>
		/// Gets the phonebook type.
		/// </summary>
		public ePhonebookType PhonebookType { get; private set; }

		/// <summary>
		/// Gets the conference manager.
		/// </summary>
		public IConferenceManager ConferenceManager { get { return m_ConferenceManager; } }

		/// <summary>
		/// Gets the metlife routing rules instance.
		/// </summary>
		public MetlifeRouting Routing { get { return m_Routing; } }

		/// <summary>
		/// Gets the path to the loaded dialing plan xml file. Used by fusion :(
		/// </summary>
		public DialingPlanInfo DialingPlan { get; private set; }

		/// <summary>
		/// Set to false when the inactivity timer elapses.
		/// </summary>
		public bool IsOccupied
		{
			get { return m_IsOccupied; }
			set
			{
				if (value == m_IsOccupied)
					return;

				m_IsOccupied = value;

				OnOccupiedStateChanged.Raise(this, new BoolEventArgs(m_IsOccupied));
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public MetlifeRoom()
		{
			m_Owner = new MetlifeRoomOwner();

			m_ShutdownTimer = new IcdTimer();
			m_ShutdownTimer.OnElapsed += ShutdownTimerOnElapsed;

			m_VolumePoints = new List<VolumePoint>();

			// Conference manager
			m_ConferenceManager = new ConferenceManager();
			Subscribe(m_ConferenceManager);

			m_InactivityTimer = SafeTimer.Stopped(InactivityTimerCallback);

			m_Routing = new MetlifeRouting(this);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnOccupiedStateChanged = null;
			OnShutdown = null;

			if (m_ShutdownTimer != null)
				m_ShutdownTimer.Dispose();
			if (m_InactivityTimer != null)
				m_InactivityTimer.Dispose();

			base.DisposeFinal(disposing);

			Unsubscribe(m_ConferenceManager);
		}

		/// <summary>
		/// Gets the display devices.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IDisplay> GetDisplays()
		{
			return Devices.OfType<IDisplay>();
		}

		/// <summary>
		/// Shuts down the room.
		/// </summary>
		[PublicAPI]
		public void Shutdown()
		{
			m_ShutdownTimer.Stop();

			// Undo all routing
			foreach (IRouteSwitcherControl switcher in this.GetControls<IRouteSwitcherControl>())
				switcher.Clear();

			m_InactivityTimer.Stop();

			// Hangup
			if (ConferenceManager.ActiveConference != null)
				ConferenceManager.ActiveConference.Hangup();

			// Power off displays
			foreach (IDisplay display in GetDisplays())
				display.PowerOff();

			OnShutdown.Raise(this);
		}

		/// <summary>
		/// Stops the shutdown timer.
		/// </summary>
		public void StopShutdownTimer()
		{
			m_ShutdownTimer.Stop();
		}

		/// <summary>
		/// Resets the shutdown timer.
		/// </summary>
		public void ResetShutdownTimer()
		{
			m_ShutdownTimer.Restart(SHUTDOWN_TIME);
		}

		/// <summary>
		/// Starts presenting the codec source.
		/// </summary>
		public void StartPresentation()
		{
			CiscoCodec codec = this.GetDevice<CiscoCodec>();
			if (codec == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to start presentation - Room has no Codec");
				return;
			}

			VideoInputConnector source = GetPresentationVideoSource();
			if (source == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to find presentation video source");
				return;
			}

			PresentationComponent presentation = codec.Components.GetComponent<PresentationComponent>();

			presentation.StartPresentation(source.SourceId, PresentationItem.eSendingMode.LocalRemote);
		}

		/// <summary>
		/// Stops the active presentation.
		/// </summary>
		public void StopPresentation()
		{
			CiscoCodec codec = this.GetDevice<CiscoCodec>();
			if (codec == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to stop presentation - Room has no Codec");
				return;
			}

			// Important we get the presentation source before ending the presentation.
			MetlifeSource source = m_Routing.GetCodecSource();

			// Stop the presentation/s
			PresentationComponent component = codec.Components.GetComponent<PresentationComponent>();
			foreach (PresentationItem presentation in component.GetPresentations())
				component.StopPresentation(presentation);

			// Now unroute everything
			if (source != null)
				m_Routing.UnrouteSource(source);
		}

		/// <summary>
		/// Gets the volume controls for the current context.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IVolumeDeviceControl> GetVolumeControls()
		{
			return m_VolumePoints.Select(v => GetVolumeControl(v));
		}

		/// <summary>
		/// Gets the best volume control for the current context.
		/// </summary>
		/// <returns></returns>
		public IVolumeDeviceControl GetBestVolumeControlForContext()
		{
			IConference conference = m_ConferenceManager.ActiveConference;
			IConferenceSource[] sources = conference == null
				                              ? new IConferenceSource[0]
				                              : conference.GetSources()
				                                          .Where(s => s.GetIsOnline())
				                                          .ToArray();

			IConferenceSource lastAudioCall = sources.Where(s => s.SourceType == eConferenceSourceType.Audio)
			                                         .OrderBy(s => s.Start)
			                                         .LastOrDefault();
			IConferenceSource lastVideoCall = sources.Where(s => s.SourceType == eConferenceSourceType.Video)
			                                         .OrderBy(s => s.Start)
			                                         .LastOrDefault();

			bool inAudioCall = lastAudioCall != null;
			bool inVideoCall = lastVideoCall != null;

			eVolumeType type;

			if (inVideoCall && inAudioCall)
				type = lastVideoCall.Start > lastAudioCall.Start ? eVolumeType.Vtc : eVolumeType.Atc;
			else if (!inAudioCall && !inVideoCall)
				type = eVolumeType.Program;
			else if (inAudioCall)
				type = eVolumeType.Atc;
			else
				type = eVolumeType.Vtc;

			VolumePoint point = GetVolumePoint(type);

			// Edge case - If no ATC or VTC defined, use program
			if (point == null && (type == eVolumeType.Atc || type == eVolumeType.Vtc))
			{
				type = eVolumeType.Program;
				point = GetVolumePoint(type);
			}

			if (point != null)
				return GetVolumeControl(point);

			Logger.AddEntry(eSeverity.Error, "Unable to find {0} with volume type {1}", typeof(VolumePoint).Name, type);
			return null;
		}

		/// <summary>
		/// Gets the first volume point matching the given volume type.
		/// </summary>
		/// <param name="volumeType"></param>
		/// <returns></returns>
		[CanBeNull]
		private VolumePoint GetVolumePoint(eVolumeType volumeType)
		{
			return m_VolumePoints.FirstOrDefault(p => p.VolumeType == volumeType);
		}

		/// <summary>
		/// Gets the volume control for the given volume point.
		/// </summary>
		/// <param name="volumePoint"></param>
		/// <returns></returns>
		private IVolumeDeviceControl GetVolumeControl(VolumePoint volumePoint)
		{
			if (volumePoint == null)
				throw new ArgumentNullException("volumePoint");

			IDevice device = Devices.GetInstance(volumePoint.DeviceId);

			IVolumeDeviceControl output = volumePoint.ControlId.HasValue
				                              ? device.Controls.GetControl<IVolumeDeviceControl>(volumePoint.ControlId.Value)
				                              : device.Controls.GetControl<IVolumeDeviceControl>();

			if (output == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to find {0} for {1}(DeviceId={2}, ControlId={3}, VolumeType={4})",
				                  typeof(IVolumeDeviceControl).Name, typeof(VolumePoint), volumePoint.DeviceId,
				                  volumePoint.ControlId, volumePoint.VolumeType);
			}

			return output;
		}

		/// <summary>
		/// Resets the inactivity timer.
		/// </summary>
		public void ResetInactivityTimer()
		{
			m_InactivityTimer.Reset(m_InactivitySeconds * 1000);
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(MetlifeRoomSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Prefix = Prefix;
			settings.Number = Number;

			settings.OwnerName = m_Owner.Name;
			settings.OwnerPhone = m_Owner.Phone;
			settings.OwnerEmail = m_Owner.Email;

			settings.DialingPlan = DialingPlan;
			settings.PhonebookType = PhonebookType;
			settings.TvPresets = m_TvPresetsPath;
			settings.InactivitySeconds = m_InactivitySeconds;

			settings.SetVolumePoints(m_VolumePoints);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Prefix = null;
			Number = null;
			PhoneNumber = null;

			// Owner
			m_Owner.Name = null;
			m_Owner.Email = null;
			m_Owner.Phone = null;

			Config = null;
			Prefix = null;
			DialingPlan = default(DialingPlanInfo);
			PhonebookType = default(ePhonebookType);
			m_TvPresetsPath = null;

			m_VolumePoints.Clear();

			m_Routing.SetRouteThroughCodec(false);

			m_ConferenceManager.ClearDialingProviders();
			m_ConferenceManager.Favorites = null;
			m_ConferenceManager.DialingPlan.ClearMatchers();
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(MetlifeRoomSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_InactivitySeconds = settings.InactivitySeconds;

			Prefix = settings.Prefix;
			Number = settings.Number;
			PhoneNumber = settings.PhoneNumber;

			// Owner
			m_Owner.Name = settings.OwnerName;
			m_Owner.Phone = settings.OwnerPhone;
			m_Owner.Email = settings.OwnerEmail;

			// Dialing plan
			SetDialingPlan(settings.DialingPlan);

			// Favorites
			string favoritesName = string.Format("{0} - {1}{2}", Name, Prefix, SqLiteFavorites.SQLITE_EXT);
			string favoritesPath = PathUtils.GetProgramConfigPath(favoritesName);
			m_ConferenceManager.Favorites = new SqLiteFavorites(favoritesPath);

			// Directory type
			PhonebookType = settings.PhonebookType;

			// Tv Presets
			SetTvPresetsFromPath(settings.TvPresets);

			// Build sources and destinations
			SetVolumePoints(settings.GetVolumePoints());
			if (Destinations.Count == 0)
				BuildDestinations();

			// Set the initial routing destination
			m_Routing.SetRouteThroughCodec(false);
		}

		/// <summary>
		/// Migration step. V4 rooms do not have destinations in their configs.
		/// </summary>
		private void BuildDestinations()
		{
			IDisplay[] displays = Devices.GetInstances<IDisplay>().ToArray();

			for (int index = 0; index < displays.Length; index++)
			{
				int destinationId = MathUtils.GetNewId(Core.GetRoutingGraph().Destinations.Select(d => d.Id));

				IDisplay display = displays[index];
				IRouteDestinationControl control = display.Controls.GetControl<IRouteDestinationControl>();
				int controlId = control == null ? 0 : control.Id;
				int input = control == null
					            ? 1
								: Core.GetRoutingGraph()
					                  .Connections
					                  .GetInputs(control, eConnectionType.Video)
					                  .FirstOrDefault(1);

				MetlifeDestination.eVtcOption vtcOption = index % 2 == 0
					                                          ? MetlifeDestination.eVtcOption.Main
					                                          : MetlifeDestination.eVtcOption.Secondary;
				MetlifeDestination.eAudioOption audioOption = MetlifeDestination.eAudioOption.Program &
				                                              MetlifeDestination.eAudioOption.Call;

				MetlifeDestination destination = new MetlifeDestination
				{
					Id = destinationId,
					Endpoint = new EndpointInfo(display.Id, controlId, input),
					ShareByDefault = true,
					VtcOption = vtcOption,
					AudioOption = audioOption
				};

				Core.GetRoutingGraph().Destinations.AddChild(destination);
				Destinations.Add(destination.Id);
			}
		}

		/// <summary>
		/// Sets the volume points from settings. Generates volume points if the sequence is empty.
		/// </summary>
		/// <param name="volumePoints"></param>
		private void SetVolumePoints(IEnumerable<VolumePoint> volumePoints)
		{
			List<VolumePoint> volumePointsList = new List<VolumePoint>(volumePoints);

			// Migration - generate destinations from the codec and displays
			if (volumePointsList.Count == 0)
			{
				volumePointsList.AddRange(this.GetControls<IVolumeDeviceControl>()
				                              .Select(control => new VolumePoint(control.Parent.Id, control.Id, eVolumeType.Program)));
			}

			m_VolumePoints.Clear();
			m_VolumePoints.AddRange(volumePointsList);
		}

		/// <summary>
		/// Sets the dialing plan from the given xml document path.
		/// </summary>
		/// <param name="planInfo"></param>
		private void SetDialingPlan(DialingPlanInfo planInfo)
		{
			DialingPlan = planInfo;

			// TODO - Move loading from path into the DialingPlan.
			string dialingPlanPath = string.IsNullOrEmpty(DialingPlan.ConfigPath)
										 ? null
										 : PathUtils.GetDefaultConfigPath("DialingPlans", DialingPlan.ConfigPath);

			try
			{
				string dialingPlanXml = IcdFile.ReadToEnd(dialingPlanPath, Encoding.ASCII);
				m_ConferenceManager.DialingPlan.LoadMatchersFromXml(dialingPlanXml);
			}
			catch (Exception e)
			{
				Logger.AddEntry(eSeverity.Error, e, "Failed to load Dialing Plan {0} - {1}", dialingPlanPath, e.Message);
			}

			// Migration - If there are no audio or video providers, search the available controls
			if (DialingPlan.VideoEndpoint.DeviceId == 0 && DialingPlan.AudioEndpoint.DeviceId == 0)
			{
				IDialingDeviceControl[] dialers = this.GetControls<IDialingDeviceControl>().ToArray();

				DeviceControlInfo video = dialers.Where(d => d.Supports == eConferenceSourceType.Video)
				                                 .Select(d => new DeviceControlInfo(d))
				                                 .FirstOrDefault();

				DeviceControlInfo audio = dialers.Where(d => d.Supports == eConferenceSourceType.Audio)
												 .Select(d => new DeviceControlInfo(d))
												 .FirstOrDefault(video);

				DialingPlan = new DialingPlanInfo(DialingPlan.ConfigPath, video, audio);
			}			

			// Setup the dialing providers
			if (DialingPlan.VideoEndpoint.DeviceId != 0)
				m_ConferenceManager.RegisterDialingProvider(eConferenceSourceType.Video,
				                                            this.GetControl<IDialingDeviceControl>(DialingPlan.VideoEndpoint));

			if (DialingPlan.AudioEndpoint.DeviceId != 0)
				m_ConferenceManager.RegisterDialingProvider(eConferenceSourceType.Audio,
															this.GetControl<IDialingDeviceControl>(DialingPlan.AudioEndpoint));
		}

		/// <summary>
		/// Sets the tv presets from the given xml document path.
		/// </summary>
		/// <param name="path"></param>
		private void SetTvPresetsFromPath(string path)
		{
			m_TvPresetsPath = path;

			XmlTvPresets presets = new XmlTvPresets();
			TvPresets = presets;

			string tvPresetsPath = string.IsNullOrEmpty(path)
				                       ? null
				                       : PathUtils.GetDefaultConfigPath("TvPresets", m_TvPresetsPath);

			if (string.IsNullOrEmpty(tvPresetsPath))
				return;

			try
			{
				string tvPresetsXml = IcdFile.ReadToEnd(tvPresetsPath, Encoding.ASCII);
				presets.Parse(tvPresetsXml);
			}
			catch (Exception e)
			{
				Logger.AddEntry(eSeverity.Error, "Failed to load TV Presets {0} - {1}", m_TvPresetsPath, e.Message);
			}
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Shutdown timer running", m_ShutdownTimer.IsRunning);
			addRow("Shutdown timer remaining", m_ShutdownTimer.RemainingSeconds);

			addRow("Prefix", Prefix);
			addRow("Config", Config);
			addRow("Number", Number);
			addRow("Phone Number", PhoneNumber);
			addRow("Owner", Owner);
			addRow("Phonebook Type", PhonebookType);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called when the shutdown timer elapses.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ShutdownTimerOnElapsed(object sender, EventArgs eventArgs)
		{
			Shutdown();
			ShutdownTimer.Stop();
		}

		/// <summary>
		/// Called when the inactivity timer elapses.
		/// </summary>
		private void InactivityTimerCallback()
		{
			// Never turn off the system if we are in a call.
			if (m_ConferenceManager.IsInCall)
			{
				ResetInactivityTimer();
				return;
			}

			// Don't turn off the system if a source is actively routed to a display
			if (m_Routing.GetActiveSources().Any())
			{
				ResetInactivityTimer();
				return;
			}

			// User left the room
			IsOccupied = false;

			ResetShutdownTimer();
		}

		/// <summary>
		/// Gets the source for a presentation.
		/// </summary>
		/// <returns></returns>
		private VideoInputConnector GetPresentationVideoSource()
		{
			CiscoCodec codec = this.GetDevice<CiscoCodec>();
			if (codec == null)
				throw new NotSupportedException("Unable to get presentation video source - Room has no Codec");

			int input = m_Routing.GetCodecInput();

			VideoComponent video = codec.Components.GetComponent<VideoComponent>();
			return video.GetVideoInputConnector(input);
		}

		#endregion

		#region Conference Manager Callbacks

		/// <summary>
		/// Subscribe to the conference manager events.
		/// </summary>
		/// <param name="conferenceManager"></param>
		private void Subscribe(IConferenceManager conferenceManager)
		{
			conferenceManager.OnInVideoCallChanged += ConferenceManagerOnInVideoCallChanged;
			conferenceManager.OnRecentConferenceAdded += ConferenceManagerOnRecentConferenceAdded;
			conferenceManager.OnActiveConferenceStatusChanged += ConferenceManagerOnActiveConferenceStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference manager events.
		/// </summary>
		/// <param name="conferenceManager"></param>
		private void Unsubscribe(IConferenceManager conferenceManager)
		{
			conferenceManager.OnInVideoCallChanged -= ConferenceManagerOnInVideoCallChanged;
			conferenceManager.OnRecentConferenceAdded -= ConferenceManagerOnRecentConferenceAdded;
			conferenceManager.OnActiveConferenceStatusChanged -= ConferenceManagerOnActiveConferenceStatusChanged;
		}

		/// <summary>
		/// Called when the active conference changes status.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnActiveConferenceStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			ConferenceStatusChanged(args.Data);
		}

		/// <summary>
		/// Called when a conference is added to the conference manager.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnRecentConferenceAdded(object sender, ConferenceEventArgs args)
		{
			ConferenceStatusChanged(args.Data.Status);
		}

		/// <summary>
		/// Called when a conference status changes.
		/// </summary>
		/// <param name="status"></param>
		private void ConferenceStatusChanged(eConferenceStatus status)
		{
			switch (status)
			{
				case eConferenceStatus.Connecting:
				case eConferenceStatus.Connected:
					m_Routing.SetRouteThroughCodec(true);
					break;

				case eConferenceStatus.Disconnected:
					m_Routing.SetRouteThroughCodec(false);
					break;
			}
		}

		/// <summary>
		/// Called when we enter/leave a video call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnInVideoCallChanged(object sender, BoolEventArgs args)
		{
			CiscoCodec codec = this.GetDevice<CiscoCodec>();
			if (codec == null)
				return;

			VideoComponent video = codec.Components.GetComponent<VideoComponent>();
			video.SetSelfViewEnabled(args.Data);
		}

		#endregion
	}
}
