using System.Linq;
using ICD.Connect.Displays;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Settings.Core;
using ICD.Common.Properties;
using ICD.Common.EventArguments;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class DisplaysFusionPresenter : AbstractFusionPresenter<IDisplaysFusionView>, IDisplaysFusionPresenter
	{
		private IDisplay m_LeftDisplay;
		private IDisplay m_RightDisplay;
		private IRouteSourceControl m_LeftReceiver;
		private IRouteSourceControl m_RightReceiver;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public DisplaysFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
			: base(roomId, presenters, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();

			bool leftDisplayOnline = m_LeftDisplay != null && m_LeftDisplay.IsOnline;
			bool leftDisplayReceiverOnline = m_LeftReceiver != null && m_LeftReceiver.Parent.IsOnline;
			string leftDisplayReceiverType = m_LeftReceiver == null ? string.Empty : m_LeftReceiver.GetType().Name;
			string leftDisplayReceiverVersion = string.Empty;

			bool rightDisplayReceiverOnline = m_RightReceiver != null && m_RightReceiver.Parent.IsOnline;
			string rightDisplayReceiverType = m_RightReceiver == null ? string.Empty : m_RightReceiver.GetType().Name;
			string rightDisplayReceiverVersion = string.Empty;

			GetView().SetLeftDisplayOnline(leftDisplayOnline);
			GetView().SetLeftDisplayReceiverOnline(leftDisplayReceiverOnline);
			GetView().SetLeftDisplayReceiverType(leftDisplayReceiverType);
			GetView().SetLeftDisplayReceiverFirmwareVersion(leftDisplayReceiverVersion);

			GetView().SetRightDisplayReceiverOnline(rightDisplayReceiverOnline);
			GetView().SetRightDisplayReceiverType(rightDisplayReceiverType);
			GetView().SetRightDisplayReceiverFirmwareVersion(rightDisplayReceiverVersion);
		}

		#region Private Methods

		/// <summary>
		/// Returns the receiver for the given display.
		/// </summary>
		/// <param name="display"></param>
		/// <returns></returns>
		[CanBeNull]
		private IRouteSourceControl GetReceiver(IDisplay display)
		{
			if (display == null)
				return null;

			IRouteDestinationControl control = display.Controls.GetControl<IRouteDestinationControl>();

			return Core.GetRoutingGraph()
			           .GetSourceControlsRecursive(control, eConnectionType.Video)
			           .FirstOrDefault(IsReceiver);
		}

		/// <summary>
		/// Returns true if the source device is a throughput and not a switcher.
		/// </summary>
		/// <param name="sourceDevice"></param>
		/// <returns></returns>
		private static bool IsReceiver(IRouteSourceControl sourceDevice)
		{
			return sourceDevice is IRouteDestinationControl && !(sourceDevice is IRouteSwitcherControl);
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

			m_LeftDisplay = room == null ? null : room.GetDisplays().FirstOrDefault();
			m_RightDisplay = room == null ? null : room.GetDisplays().Skip(1).FirstOrDefault();
			m_LeftReceiver = GetReceiver(m_LeftDisplay);
			m_RightReceiver = GetReceiver(m_RightDisplay);

			if (m_LeftDisplay != null)
				m_LeftDisplay.OnIsOnlineStateChanged += LeftDisplayOnIsOnlineStateChanged;
			if (m_RightDisplay != null)
				m_RightDisplay.OnIsOnlineStateChanged += RightDisplayOnIsOnlineStateChanged;

			if (m_LeftReceiver != null)
				m_LeftReceiver.Parent.OnIsOnlineStateChanged += LeftReceiverOnIsOnlineStateChanged;
			if (m_RightReceiver != null)
				m_RightReceiver.Parent.OnIsOnlineStateChanged += RightReceiverOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (m_LeftDisplay != null)
				m_LeftDisplay.OnIsOnlineStateChanged -= LeftDisplayOnIsOnlineStateChanged;
			if (m_RightDisplay != null)
				m_RightDisplay.OnIsOnlineStateChanged -= RightDisplayOnIsOnlineStateChanged;

			if (m_LeftReceiver != null)
				m_LeftReceiver.Parent.OnIsOnlineStateChanged -= LeftReceiverOnIsOnlineStateChanged;
			if (m_RightReceiver != null)
				m_RightReceiver.Parent.OnIsOnlineStateChanged -= RightReceiverOnIsOnlineStateChanged;
		}

		private void RightReceiverOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshAsync();
		}

		private void LeftReceiverOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshAsync();
		}

		private void RightDisplayOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshAsync();
		}

		private void LeftDisplayOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshAsync();
		}

		#endregion
	}
}
