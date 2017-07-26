using System;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.MainNav.Components
{
	/// <summary>
	/// Base class for main nav components that lead to a menu.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AbstractMainNavMenuComponentPresenter<T> : AbstractMainNavComponentPresenter
		where T : class, IPresenter
	{
		private T m_CachedMenu;

		/// <summary>
		/// Gets the menu for this button.
		/// </summary>
		protected T Menu { get { return m_CachedMenu ?? (m_CachedMenu = Navigation.LazyLoadPresenter<T>()); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractMainNavMenuComponentPresenter(int room, INavigationController nav,
		                                                IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			Subscribe(Menu);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			if (m_CachedMenu != null)
				Unsubscribe(m_CachedMenu);

			base.Dispose();
		}

		#region Protected Methods

		/// <summary>
		/// Gets the current icon state for the component.
		/// </summary>
		/// <returns></returns>
		protected override eIconState GetIconState()
		{
			bool visible = m_CachedMenu != null && Menu.IsViewVisible;
			return visible ? eIconState.Active : eIconState.Default;
		}

		/// <summary>
		/// Called when the component is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			Menu.ShowView(!Menu.IsViewVisible);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// 
		/// If the nav item is hidden, hide the menu.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			base.ViewOnVisibilityChanged(sender, boolEventArgs);

			if (m_CachedMenu != null && !boolEventArgs.Data)
				Menu.ShowView(false);
		}

		#endregion

		#region Navigation Callbacks

		/// <summary>
		/// Subscribe to the menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Subscribe(T menu)
		{
			menu.OnViewVisibilityChanged += MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Unsubscribe(T menu)
		{
			menu.OnViewVisibilityChanged -= MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Called when the menu visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void MenuOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
