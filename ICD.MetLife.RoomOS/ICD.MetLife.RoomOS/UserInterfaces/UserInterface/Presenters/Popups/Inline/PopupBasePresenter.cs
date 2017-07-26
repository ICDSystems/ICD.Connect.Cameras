using System;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline
{
	public sealed class PopupBasePresenter : AbstractPresenter<IPopupBaseView>, IPopupBasePresenter
	{
		private IPresenter m_Menu;
		private string m_Title;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public PopupBasePresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IPopupBaseView view)
		{
			base.Refresh(view);

			view.SetTitle(m_Title ?? string.Empty);
		}

		/// <summary>
		/// Sets the current menu for the presenter.
		/// </summary>
		/// <param name="menu"></param>
		/// <param name="title"></param>
		public void SetMenu(IPresenter menu, string title)
		{
			if (menu == m_Menu && title == m_Title)
				return;

			Unsubscribe(m_Menu);
			m_Menu = menu;
			Subscribe(m_Menu);

			m_Title = title;

			RefreshIfVisible();
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IPopupBaseView view)
		{
			base.Subscribe(view);

			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IPopupBaseView view)
		{
			base.Unsubscribe(view);

			view.OnCloseButtonPressed -= ViewOnCloseButtonPressed;
		}

		/// <summary>
		/// Called when the close button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
			if (m_Menu != null)
				m_Menu.ShowView(false);
		}

		#endregion

		#region Menu Callbacks

		/// <summary>
		/// Subscribe to the menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Subscribe(IPresenter menu)
		{
			if (menu == null)
				return;

			menu.OnViewVisibilityChanged += MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Unsubscribe(IPresenter menu)
		{
			if (menu == null)
				return;

			menu.OnViewVisibilityChanged -= MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Called when the menu visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void MenuOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			// Hide the presenter if the menu is hidden.
			if (!boolEventArgs.Data)
				ShowView(false);
		}

		#endregion
	}
}
