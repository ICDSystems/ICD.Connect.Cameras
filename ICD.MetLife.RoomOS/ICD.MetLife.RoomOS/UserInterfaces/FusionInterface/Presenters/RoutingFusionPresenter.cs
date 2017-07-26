using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Settings.Core;
using ICD.Common.Properties;
using ICD.Connect.Sources.TvTuner;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;
using ICD.Connect.Conferencing.Cisco;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class RoutingFusionPresenter : AbstractFusionPresenter<IRoutingFusionView>, IRoutingFusionPresenter
	{
		private IRouteSourceControl m_FrontInput;
		private IRouteSourceControl m_WirelessInput;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public RoutingFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
			: base(roomId, presenters, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();

			bool frontInputAvailable = m_FrontInput != null;

			bool frontInputSync = m_FrontInput != null && Core.GetRoutingGraph().SourceDetected(m_FrontInput, eConnectionType.Video);
			bool wirelessInputSync = m_WirelessInput != null &&
									 Core.GetRoutingGraph().SourceDetected(m_WirelessInput, eConnectionType.Video);

			string frontInputVideoType = string.Empty; // todo
			string frontInputHdmiResolution = string.Empty; // todo
			string frontInputVgaResolution = string.Empty; // todo

			string rearInputVideoType = string.Empty; // todo
			string rearInputHdmiResolution = string.Empty; // todo
			string rearInputVgaResolution = string.Empty; // todo

			ITvTuner tvTuner = Room.GetDevice<ITvTuner>();
			IRouteSourceControl tvTunerSourceControl = tvTuner == null ? null : tvTuner.Controls.GetControl<IRouteSourceControl>();
			bool tvTunerSync = tvTunerSourceControl != null && Core.GetRoutingGraph().SourceDetected(tvTunerSourceControl, eConnectionType.Video);

			CiscoCodec codec = Room.GetDevice<CiscoCodec>();
			bool conferencingMonitor1Sync = codec != null && Core.GetRoutingGraph().SourceDetected(codec.Controls.GetControl<IRouteSourceControl>(), eConnectionType.Video);

			string frontTransmitterType = string.Empty; // todo
			string frontTransmitterFirmwareVersion = string.Empty; // todo
			string rearTransmitterType = string.Empty; // todo
			string rearTransmitterFirmwareVersion = string.Empty; // todo

			GetView().SetFrontInputAvailable(frontInputAvailable);

			GetView().SetFrontInputSync(frontInputSync);
			GetView().SetWirelessInputSync(wirelessInputSync);

			GetView().SetFrontInputVideoType(frontInputVideoType);
			GetView().SetFrontInputHdmiResolution(frontInputHdmiResolution);
			GetView().SetFrontInputVgaResolution(frontInputVgaResolution);

			GetView().SetRearInputVideoType(rearInputVideoType);
			GetView().SetRearInputHdmiResolution(rearInputHdmiResolution);
			GetView().SetRearInputVgaResolution(rearInputVgaResolution);

			GetView().SetTvTunerSync(tvTunerSync);
			GetView().SetVideoConferencingMonitor1Sync(conferencingMonitor1Sync);

			GetView().SetFrontTransmitterType(frontTransmitterType);
			GetView().SetFrontTransmitterFirmwareVersion(frontTransmitterFirmwareVersion);
			GetView().SetRearTransmitterType(rearTransmitterType);
			GetView().SetRearTransmitterFirmwareVersion(rearTransmitterFirmwareVersion);
		}

		#region Private Methods

		/// <summary>
		/// Gets the front input source device for the room. This is a total guess until we figure out a better way of
		/// getting specific inputs while keeping the room nice and generic.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IRouteSourceControl GetFrontInput()
		{
			return GetInputs().FirstOrDefault();
		}

		/// <summary>
		/// Gets the wireless input source device for the room. This is a total guess until we figure out a better way of
		/// getting specific inputs while keeping the room nice and generic.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private IRouteSourceControl GetWirelessInput()
		{
			return GetInputs().Skip(2).FirstOrDefault();
		}

		/// <summary>
		/// Loops over the inputs for the displays.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IRouteSourceControl> GetInputs()
		{
			return Room.GetDisplays()
			           .Select(d => d.Controls.GetControl<IRouteDestinationControl>())
			           .SelectMany(c => Core.GetRoutingGraph().GetSourceControlsRecursive(c, eConnectionType.Video));
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(MetlifeRoom room)
		{
			base.Subscribe(room);

			m_FrontInput = GetFrontInput();
			m_WirelessInput = GetWirelessInput();

			if (room == null)
				return;

			room.Routing.OnSourceDetectionStateChanged += RoomOnSourceDetectionStateChanged;
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

			room.Routing.OnSourceDetectionStateChanged -= RoomOnSourceDetectionStateChanged;
		}

		private void RoomOnSourceDetectionStateChanged(object sender, EventArgs eventArgs)
		{
			RefreshAsync();
		}

		#endregion
	}
}
