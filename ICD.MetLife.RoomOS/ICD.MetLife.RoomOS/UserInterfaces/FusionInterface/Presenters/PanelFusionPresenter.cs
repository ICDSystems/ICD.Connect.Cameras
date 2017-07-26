using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class PanelFusionPresenter : AbstractFusionPresenter<IPanelFusionView>, IPanelFusionPresenter
	{
		private IPanelDevice m_Panel;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public PanelFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
			: base(roomId, presenters, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();

			bool online = m_Panel != null && m_Panel.IsOnline;
			string type = m_Panel == null ? string.Empty : m_Panel.GetType().Name;
			string version = string.Empty; // todo
			string headerPath = string.Empty;
			string backgroundPath = string.Empty;

			GetView().SetPanelOnline(online);
			GetView().SetPanelType(type);
			GetView().SetPanelFirmwareVersion(version);
			GetView().SetPanelHeaderImagePath(headerPath);
			GetView().SetPanelBackgroundImagePath(backgroundPath);
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

			m_Panel = room.Panels.FirstOrDefault();

			if (m_Panel != null)
				m_Panel.OnIsOnlineStateChanged += PanelOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (m_Panel != null)
				m_Panel.OnIsOnlineStateChanged -= PanelOnIsOnlineStateChanged;

			m_Panel = null;
		}

		/// <summary>
		/// Called when the panel online state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void PanelOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshAsync();
		}

		#endregion
	}
}
