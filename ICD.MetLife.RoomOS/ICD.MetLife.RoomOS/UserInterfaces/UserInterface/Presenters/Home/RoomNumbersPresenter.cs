using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Components.System;
using ICD.Connect.Displays;
using ICD.Connect.Rooms;
using ICD.Connect.Rooms.Extensions;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Home
{
	public sealed class RoomNumbersPresenter : AbstractPresenter<IRoomNumbersView>, IRoomNumbersPresenter
	{
		private SystemComponent m_System;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public RoomNumbersPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IRoomNumbersView view)
		{
			base.Refresh(view);

			string videoNumber = m_System == null ? string.Empty : m_System.SipUri;
			string audioNumber = (Room == null ? null : Room.PhoneNumber) ?? string.Empty;
			string sharingStatus = GetSharingStatus();

			view.SetVideoNumber(videoNumber);
			view.ShowVideoLabel(!string.IsNullOrEmpty(videoNumber));

			view.SetAudioNumber(audioNumber);
			view.ShowAudioLabel(!string.IsNullOrEmpty(audioNumber));

			view.SetSharingStatusText(sharingStatus);
		}

		/// <summary>
		/// The text in the top right that has sharing status should read
		/// For a single source “Sharing $source$”
		/// For multiple sources “Sharing $source$ and $source$” (continue as necessary)
		/// Should be in the same order the displays are in.
		/// </summary>
		/// <returns></returns>
		private string GetSharingStatus()
		{
			string[] activeSourceNames = GetDestinationEndpoints()
				.Select<EndpointInfo, string>(GetDestinationEndpointSharingLabel)
				.Where(s => s != null)
				.Consolidate()
				.ToArray();

			if (activeSourceNames.Length == 0)
				return string.Empty;

			string sourcesCommas = StringUtils.SerialComma(activeSourceNames);
			return string.Format("Sharing {0}", sourcesCommas);
		}

		/// <summary>
		/// Enumerates over the display endpoints in the room.
		/// Prepends with the codec input if in a video call.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<EndpointInfo> GetDestinationEndpoints()
		{
			if (Room == null)
				return Enumerable.Empty<EndpointInfo>();

			IEnumerable<EndpointInfo> output =
				Room.Routing
				    .GetDestinations()
				    .Where(d => Room.Devices[d.Endpoint.Device] is IDisplay)
				    .Select(d => d.Endpoint);

			if (!Room.ConferenceManager.IsInVideoCall)
				return output;

			CiscoCodec codec = Room.GetDevice<CiscoCodec>();
			if (codec == null)
				return output;

			EndpointInfo input = Room.Routing.GetCodecInputEndpointInfo();
			return output.Prepend(input);
		}

		/// <summary>
		/// Gets the source sharing label for the given destination endpoint.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		private string GetDestinationEndpointSharingLabel(EndpointInfo endpoint)
		{
			MetlifeSource source = GetActiveSourceForDestinationEndpoint(endpoint);
			if (source == null)
				return null;

			string deviceName = source.GetNameOrDeviceName(Room);

			CiscoCodec codec = Room.GetDevice<CiscoCodec>();
			if (codec == null)
				return deviceName;

			// If in a video call and the endpoint is not the codec, append "(Private)"
			if (Room != null && Room.ConferenceManager.IsInVideoCall && endpoint.Device != codec.Id)
				deviceName = string.Format("{0} (Private)", deviceName);

			return deviceName;
		}

		/// <summary>
		/// Gets the source currently routed to the given destination input.
		/// </summary>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		private MetlifeSource GetActiveSourceForDestinationEndpoint(EndpointInfo endpoint)
		{
			return Room == null ? null : Room.Routing.GetActiveSource(endpoint, eConnectionType.Video, true);
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(MetlifeRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.Routing.OnRouteChanged += RoutingOnRouteChanged;
			room.Routing.OnSourceDetectionStateChanged += RoutingOnSourceDetectionStateChanged;
			room.Routing.OnSourceTransmissionStateChanged += RoutingOnSourceTransmissionStateChanged;
			room.ConferenceManager.OnInVideoCallChanged += ConferenceManagerOnInVideoCallChanged;

			CiscoCodec codec = room.GetDevice<CiscoCodec>();
			if (codec == null)
				return;

			m_System = codec.Components.GetComponent<SystemComponent>();
			m_System.OnSipUriChange += SystemOnSipUriChange;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.Routing.OnRouteChanged -= RoutingOnRouteChanged;
			room.Routing.OnSourceDetectionStateChanged -= RoutingOnSourceDetectionStateChanged;
			room.Routing.OnSourceTransmissionStateChanged -= RoutingOnSourceTransmissionStateChanged;
			room.ConferenceManager.OnInVideoCallChanged -= ConferenceManagerOnInVideoCallChanged;

			if (m_System == null)
				return;

			m_System.OnSipUriChange -= SystemOnSipUriChange;
		}

		/// <summary>
		/// Called when the codec SIP URI changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void SystemOnSipUriChange(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		private void RoutingOnSourceTransmissionStateChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		private void RoutingOnSourceDetectionStateChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		private void RoutingOnRouteChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		private void ConferenceManagerOnInVideoCallChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
