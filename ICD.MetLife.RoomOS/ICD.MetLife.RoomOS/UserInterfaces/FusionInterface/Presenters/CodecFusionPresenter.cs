using System;
using System.Linq;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Components.Cameras;
using ICD.Connect.Conferencing.Cisco.Components.System;
using ICD.Connect.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Settings.Core;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class CodecFusionPresenter : AbstractFusionPresenter<ICodecFusionView>, ICodecFusionPresenter
	{
		private SystemComponent m_System;
		private NearCamerasComponent m_Cameras;
		private IRouteSwitcherControl m_PresentationSwitcher;
		private CiscoCodec m_Codec;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public CodecFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
			: base(roomId, presenters, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();

			bool hasVideoConferencing = false;
			bool videoConferencingOnline = false;
			bool cameraOnline = false;
			bool switcherOnline = false;
			string systemName = string.Empty;
			string ipAddress = string.Empty;
			string defaultGateway = string.Empty;
			string subnetMask = string.Empty;
			string gatekeeperStatus = string.Empty;
			string gatekeeperMode = string.Empty;
			string gatekeeperAddress = string.Empty;
			string h323Id = string.Empty;
			string e164Alias = string.Empty;
			string sipUri = string.Empty;
			string sipProxyAddress = string.Empty;
			string sipProxyStatus = string.Empty;
			string softwareVersion = string.Empty;

			if (m_Codec != null)
			{
				hasVideoConferencing = true;
				videoConferencingOnline = m_Codec.IsOnline;
			}

			if (m_Cameras != null)
				cameraOnline = m_Cameras.CamerasCount > 0;

			if (m_PresentationSwitcher != null)
				switcherOnline = m_PresentationSwitcher.Parent.IsOnline;

			if (m_System != null)
			{
				systemName = m_System.Name;
				ipAddress = m_System.Address;
				defaultGateway = m_System.Gateway;
				subnetMask = m_System.SubnetMask;
				gatekeeperStatus = StringUtils.NiceName(m_System.H323GatekeeperStatus);
				gatekeeperMode = string.Empty; // todo
				gatekeeperAddress = m_System.H323GatekeeperAddress;
				h323Id = string.Empty; // todo
				e164Alias = string.Empty; // todo
				sipUri = m_System.SipUri;
				sipProxyAddress = m_System.SipProxyAddress;
				sipProxyStatus = m_System.SipProxyStatus;
				softwareVersion = m_System.SoftwareVersion;
			}

			GetView().SetHasVideoConferencing(hasVideoConferencing);
			GetView().SetVideoConferencingOnline(videoConferencingOnline);
			GetView().SetVtcCameraOnline(cameraOnline);
			GetView().SetVtcPresentationSwitcherOnline(switcherOnline);
			GetView().SetVtcSystemName(systemName);
			GetView().SetVtcIpAddress(ipAddress);
			GetView().SetVtcDefaultGateway(defaultGateway);
			GetView().SetVtcSubnetMask(subnetMask);
			GetView().SetVtcGatekeeperStatus(gatekeeperStatus);
			GetView().SetVtcGatekeeperMode(gatekeeperMode);
			GetView().SetVtcGatekeeperAddress(gatekeeperAddress);
			GetView().SetVtcH323Id(h323Id);
			GetView().SetVtcE164Alias(e164Alias);
			GetView().SetVtcSipUri(sipUri);
			GetView().SetVtcSipProxyAddress(sipProxyAddress);
			GetView().SetVtcSipProxyStatus(sipProxyStatus);
			GetView().SetVtcSoftwareVersion(softwareVersion);
		}

		/// <summary>
		/// Finds the first switcher pathed to the codec.
		/// </summary>
		/// <returns></returns>
		private IRouteSwitcherControl GetPresentationSwitcher(IRouteDestinationControl destination)
		{
			return destination == null
				       ? null
				       : Core.GetRoutingGraph()
				             .GetSourceControlsRecursive(destination, eConnectionType.Video)
				             .OfType<IRouteSwitcherControl>()
				             .FirstOrDefault();
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(MetlifeRoom room)
		{
			base.Subscribe(room);

			m_Codec = room == null ? null : room.GetDevice<CiscoCodec>();
			m_System = m_Codec == null ? null : m_Codec.Components.GetComponent<SystemComponent>();
			m_Cameras = m_Codec == null ? null : m_Codec.Components.GetComponent<NearCamerasComponent>();
			m_PresentationSwitcher = m_Codec == null ? null : GetPresentationSwitcher(m_Codec.Controls.GetControl<IRouteDestinationControl>());

			if (m_Codec != null)
				m_Codec.OnIsOnlineStateChanged += CodecOnIsOnlineStateChanged;

			if (m_Cameras != null)
				m_Cameras.OnCamerasChanged += CamerasOnCamerasChanged;

			if (m_PresentationSwitcher != null)
				m_PresentationSwitcher.Parent.OnIsOnlineStateChanged += PresentationSwitcherOnIsOnlineStateChanged;

			if (m_System == null)
				return;

			m_System.OnNameChanged += SystemOnNameChanged;
			m_System.OnAddressChanged += SystemOnAddressChanged;
			m_System.OnGatewayChanged += SystemOnGatewayChanged;
			m_System.OnSubnetMaskChanged += SystemOnSubnetMaskChanged;
			m_System.OnGatekeeperStatusChanged += SystemOnGatekeeperStatusChanged;
			m_System.OnGatekeeperAddressChanged += SystemOnGatekeeperAddressChanged;
			m_System.OnSipUriChange += SystemOnSipUriChange;
			m_System.OnSipProxyAddressChanged += SystemOnSipProxyAddressChanged;
			m_System.OnSipProxyStatusChanged += SystemOnSipProxyStatusChanged;
			m_System.OnSoftwareVersionChanged += SystemOnSoftwareVersionChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (m_Codec != null)
				m_Codec.OnIsOnlineStateChanged -= CodecOnIsOnlineStateChanged;

			if (m_Cameras != null)
				m_Cameras.OnCamerasChanged -= CamerasOnCamerasChanged;

			if (m_PresentationSwitcher != null)
				m_PresentationSwitcher.Parent.OnIsOnlineStateChanged -= PresentationSwitcherOnIsOnlineStateChanged;

			if (m_System == null)
				return;

			m_System.OnNameChanged -= SystemOnNameChanged;
			m_System.OnAddressChanged -= SystemOnAddressChanged;
			m_System.OnGatewayChanged -= SystemOnGatewayChanged;
			m_System.OnSubnetMaskChanged -= SystemOnSubnetMaskChanged;
			m_System.OnGatekeeperStatusChanged -= SystemOnGatekeeperStatusChanged;
			m_System.OnGatekeeperAddressChanged -= SystemOnGatekeeperAddressChanged;
			m_System.OnSipUriChange -= SystemOnSipUriChange;
			m_System.OnSipProxyAddressChanged -= SystemOnSipProxyAddressChanged;
			m_System.OnSipProxyStatusChanged -= SystemOnSipProxyStatusChanged;
			m_System.OnSoftwareVersionChanged -= SystemOnSoftwareVersionChanged;
		}

		private void CodecOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshAsync();
		}

		private void CamerasOnCamerasChanged(object sender, EventArgs eventArgs)
		{
			RefreshAsync();
		}

		/// <summary>
		/// Called when the presentation switcher goes online/offline.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void PresentationSwitcherOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshAsync();
		}

		private void SystemOnSoftwareVersionChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SystemOnSipProxyStatusChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SystemOnSipProxyAddressChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SystemOnSipUriChange(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SystemOnGatekeeperAddressChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SystemOnGatekeeperStatusChanged(object sender, GatekeeperStatusArgs gatekeeperStatusArgs)
		{
			RefreshAsync();
		}

		private void SystemOnSubnetMaskChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SystemOnGatewayChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SystemOnAddressChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SystemOnNameChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		#endregion
	}
}
