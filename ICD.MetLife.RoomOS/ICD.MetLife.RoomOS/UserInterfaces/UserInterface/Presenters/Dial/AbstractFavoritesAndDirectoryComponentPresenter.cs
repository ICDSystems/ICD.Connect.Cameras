using System;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public abstract class AbstractFavoritesAndDirectoryComponentPresenter :
		AbstractComponentPresenter<IFavoritesAndDirectoryComponentView>, IFavoritesAndDirectoryComponentPresenter
	{
		public event EventHandler OnPressed;

		private bool m_Selected;

		/// <summary>
		/// Gets/sets the component selection state.
		/// </summary>
		public bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				RefreshSelectionIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractFavoritesAndDirectoryComponentPresenter(int room, INavigationController nav,
		                                                          IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IFavoritesAndDirectoryComponentView view)
		{
			base.Refresh(view);

			bool favoriteButtonVisible = GetFavoriteButtonVisible();
			bool favorite = favoriteButtonVisible && GetIsFavorite();
			eRecentCallIconMode icon = GetIcon();
			string name = GetName();

			view.SetName(name);
			view.SetIcon(icon);

			view.ShowFavoriteButton(favoriteButtonVisible);
			view.SetFavorite(favorite);

			RefreshSelectionIfVisible();
		}

		private void RefreshSelectionIfVisible()
		{
			if (!IsViewVisible)
				return;

			IFavoritesAndDirectoryComponentView view = GetView(false);

			if (view != null)
				view.SetSelected(Selected);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns true if the component is favorited.
		/// </summary>
		/// <returns></returns>
		protected abstract bool GetIsFavorite();

		/// <summary>
		/// Returns the icon for the component.
		/// </summary>
		/// <returns></returns>
		protected abstract eRecentCallIconMode GetIcon();

		/// <summary>
		/// Returns true if the favorite button should be visible.
		/// </summary>
		/// <returns></returns>
		protected abstract bool GetFavoriteButtonVisible();

		/// <summary>
		/// Returns the name for the component.
		/// </summary>
		/// <returns></returns>
		protected abstract string GetName();

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IFavoritesAndDirectoryComponentView view)
		{
			base.Subscribe(view);

			view.OnFavoriteButtonPressed += ViewOnFavoriteButtonPressed;
			view.OnPressed += ViewOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IFavoritesAndDirectoryComponentView view)
		{
			base.Unsubscribe(view);

			view.OnFavoriteButtonPressed -= ViewOnFavoriteButtonPressed;
			view.OnPressed -= ViewOnPressed;
		}

		/// <summary>
		/// Called when the user presses the component.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the favorite button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected virtual void ViewOnFavoriteButtonPressed(object sender, EventArgs eventArgs)
		{
		}

		#endregion
	}
}
