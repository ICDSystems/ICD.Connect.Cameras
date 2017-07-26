using System;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Connect.Settings.Core;
using ICD.Common.Properties;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters
{
	/// <summary>
	/// Base class for all presenters.
	/// </summary>
	public abstract class AbstractPresenter<T> : IPresenter<T>
		where T : class, IView
	{
		public event EventHandler<BoolEventArgs> OnViewVisibilityChanged;

		private readonly INavigationController m_Navigation;
		private readonly int m_Room;
		private readonly IViewFactory m_ViewFactory;
		private readonly ICore m_Core;

		private T m_View;

		[UsedImplicitly] private object m_AsyncRefreshHandle;

		#region Properties

		/// <summary>
		/// Gets the navigation controller.
		/// </summary>
		protected INavigationController Navigation { get { return m_Navigation; } }

		/// <summary>
		/// Gets the metlife room.
		/// </summary>
		protected MetlifeRoom Room { get { return m_Core.Originators.GetChild<MetlifeRoom>(m_Room); } }

		/// <summary>
		/// Gets the view factory.
		/// </summary>
		protected IViewFactory ViewFactory { get { return m_ViewFactory; } }

		/// <summary>
		/// Gets the state of the view visibility.
		/// </summary>
		public bool IsViewVisible { get { return m_View != null && m_View.IsVisible; } }

		/// <summary>
		/// Gets the core object
		/// </summary>
		protected ICore Core { get { return m_Core; } }

		protected ILoggerService Logger
		{
			get { return ServiceProvider.TryGetService<ILoggerService>(); }
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
		{
			m_Room = room;
			m_Navigation = nav;
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
			OnViewVisibilityChanged = null;
			m_AsyncRefreshHandle = null;

			Unsubscribe(Room);

			if (m_View == null)
				return;

			Unsubscribe(m_View);
			m_View.Dispose();
		}

		/// <summary>
		/// Gets the view for this presenter.
		/// </summary>
		/// <returns></returns>
		protected T GetView()
		{
			return GetView(true);
		}

		/// <summary>
		/// Gets the view for the presenter.
		/// </summary>
		/// <param name="instantiate">When true instantiates a new view if the current view is null.</param>
		/// <returns></returns>
		protected T GetView(bool instantiate)
		{
			// Get default view from the factory
			if (m_View == null && instantiate)
			{
				T view = m_ViewFactory.GetNewView<T>();
				SetView(view);
			}

			return m_View;
		}

		/// <summary>
		/// Sets the current view to null.
		/// </summary>
		public void ClearView()
		{
			SetView(null);
		}

		/// <summary>
		/// Disposes the old view and sets the new view.
		/// </summary>
		/// <param name="view"></param>
		public virtual void SetView(T view)
		{
			if (view == m_View)
				return;

			if (m_View != null)
				Unsubscribe(m_View);

			m_View = view;

			if (m_View != null)
				Subscribe(m_View);

			RefreshIfVisible();
		}

		/// <summary>
		/// Sets the visibility of the presenters view.
		/// </summary>
		/// <param name="visible"></param>
		public void ShowView(bool visible)
		{
			// Don't bother creating the view just to hide it.
			if (m_View == null && !visible)
				return;

			GetView().Show(visible);
		}

		/// <summary>
		/// Sets the enabled state of the view.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetViewEnabled(bool enabled)
		{
			GetView().Enable(enabled);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		[PublicAPI]
		public virtual void Refresh()
		{
			T view = GetView(true);

			// Don't refresh if we currently have no view.
			if (view != null)
				Refresh(view);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected virtual void Refresh(T view)
		{
		}

		/// <summary>
		/// Refreshes the view asynchronously.
		/// </summary>
		[PublicAPI]
		public void RefreshAsync()
		{
			//Refresh();
			m_AsyncRefreshHandle = CrestronUtils.SafeInvoke(Refresh);
		}

		/// <summary>
		/// Asynchronously updates the view if it is currently visible.
		/// </summary>
		[PublicAPI]
		public void RefreshIfVisible()
		{
			RefreshIfVisible(true);
		}

		/// <summary>
		/// Updates the view if it is currently visible.
		/// </summary>
		/// <param name="async"></param>
		[PublicAPI]
		public void RefreshIfVisible(bool async)
		{
			if (!IsViewVisible)
				return;

			if (async)
				RefreshAsync();
			else
				Refresh();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected virtual void Subscribe(T view)
		{
			view.OnVisibilityChanged += ViewOnVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected virtual void Unsubscribe(T view)
		{
			view.OnVisibilityChanged -= ViewOnVisibilityChanged;
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected virtual void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			OnViewVisibilityChanged.Raise(this, new BoolEventArgs(args.Data));

			if (args.Data)
				RefreshAsync();
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
	}
}
