using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Connect.Rooms.Extensions;
using ICD.Connect.Settings.Core;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public abstract class AbstractFusionPresenter<TView> : IFusionPresenter<TView>
		where TView : class, IFusionView
	{
		private readonly int m_RoomId;
		private readonly IFusionViewFactory m_ViewFactory;
		private readonly ICore m_Core;

		private TView m_View;

		[UsedImplicitly] private object m_AsyncRefreshHandle;

		/// <summary>
		/// Gets the metlife room.
		/// </summary>
		protected MetlifeRoom Room { get { return m_Core.GetRooms().GetChild<MetlifeRoom>(m_RoomId); } }

		/// <summary>
		/// Gets the metlife room.
		/// </summary>
		protected ICore Core { get { return m_Core; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
		{
			m_RoomId = roomId;
			m_ViewFactory = views;
			m_Core = core;

			Subscribe(Room);
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			m_AsyncRefreshHandle = null;

			Unsubscribe(Room);

			if (m_View == null)
				return;

			Unsubscribe(m_View);
			m_View.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		[PublicAPI]
		public virtual void Refresh()
		{
		}

		/// <summary>
		/// Refreshes the view asynchronously.
		/// </summary>
		[PublicAPI]
		public void RefreshAsync()
		{
			m_AsyncRefreshHandle = CrestronUtils.SafeInvoke(Refresh);
		}

		/// <summary>
		/// Gets the view for this presenter.
		/// </summary>
		/// <returns></returns>
		public TView GetView()
		{
			// Get default view from the factory
			if (m_View == null)
			{
				TView view = m_ViewFactory.GetNewView<TView>();
				SetView(view);
			}

			return m_View;
		}

		/// <summary>
		/// Disposes the old view and sets the new view.
		/// </summary>
		/// <param name="view"></param>
		public virtual void SetView(TView view)
		{
			if (view == m_View)
				return;

			if (m_View != null)
				Unsubscribe(m_View);

			m_View = view;

			if (m_View != null)
				Subscribe(m_View);

			Refresh();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected virtual void Subscribe(TView view)
		{
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected virtual void Unsubscribe(TView view)
		{
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Subscribe(MetlifeRoom room)
		{
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected virtual void Unsubscribe(MetlifeRoom room)
		{
		}

		#endregion

		IFusionView IFusionPresenter.GetView()
		{
			return GetView();
		}
	}
}
