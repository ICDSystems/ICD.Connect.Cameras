using System;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial.DialNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial.DialNav;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial.DialNav.Components
{
	public abstract class AbstractDialNavComponentPresenter<T> : AbstractComponentPresenter<IDialNavComponentView>,
	                                                             IDialNavComponentPresenter
		where T : class, IPresenter
	{
		private T m_CachedMenu;

		/// <summary>
		/// Gets the menu for this button.
		/// </summary>
		public T Menu { get { return m_CachedMenu ?? (m_CachedMenu = Navigation.LazyLoadPresenter<T>()); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractDialNavComponentPresenter(int room, INavigationController nav,
		                                            IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			Subscribe(Menu);
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			if (m_CachedMenu != null)
				Unsubscribe(m_CachedMenu);

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IDialNavComponentView view)
		{
			base.Refresh(view);

			IIcon icon = GetIcon();
			eIconState state = GetIconState();
			string label = GetLabelText();

			view.SetIcon(icon, state);
			view.SetLabelText(label);
		}

		/// <summary>
		/// Sets the visibility of the associated menu.
		/// </summary>
		/// <param name="show"></param>
		public void ShowMenu(bool show)
		{
			// Don't instantiate the menu just to hide it.
			if (m_CachedMenu == null && !show)
				return;

			Menu.ShowView(show);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the label text for the component.
		/// </summary>
		/// <returns></returns>
		protected abstract string GetLabelText();

		/// <summary>
		/// Gets the icon state for the component.
		/// </summary>
		/// <returns></returns>
		private eIconState GetIconState()
		{
			return GetMenuVisible() ? eIconState.Active : eIconState.Default;
		}

		/// <summary>
		/// Gets the icon for the component.
		/// </summary>
		/// <returns></returns>
		protected abstract IIcon GetIcon();

		/// <summary>
		/// Gets the visibility of the menu.
		/// </summary>
		/// <returns></returns>
		private bool GetMenuVisible()
		{
			return m_CachedMenu != null && Menu.IsViewVisible;
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IDialNavComponentView view)
		{
			base.Subscribe(view);

			view.OnPressed += ViewOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IDialNavComponentView view)
		{
			base.Unsubscribe(view);

			view.OnPressed -= ViewOnPressed;
		}

		/// <summary>
		/// Called when the user presses the component.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			Menu.ShowView(!GetMenuVisible());
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
