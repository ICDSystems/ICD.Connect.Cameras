using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.Favorites;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public sealed class FavoritesPresenter : AbstractDialPresenter<IFavoritesView>,
	                                         IFavoritesPresenter
	{
		private readonly FavoritesComponentPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		private Favorite m_Selected;

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Favorites"; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public FavoritesPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_ChildrenFactory = new FavoritesComponentPresenterFactory(nav, ItemFactory);
			m_RefreshSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			UnsubscribeChildren();
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IFavoritesView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				UnsubscribeChildren();

				IEnumerable<Favorite> favorites = Room == null
					                                  ? Enumerable.Empty<Favorite>()
					                                  : Room.ConferenceManager.Favorites.GetFavorites();

				foreach (IFavoritesComponentPresenter presenter in m_ChildrenFactory.BuildChildren(favorites))
				{
					Subscribe(presenter);

					presenter.Selected = presenter.Favorite == m_Selected;
					presenter.ShowView(true);
				}

				RefreshDialButton();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Generates the given number of child views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IFavoritesAndDirectoryComponentView> ItemFactory(ushort count)
		{
			return GetView().GetChildCallViews(ViewFactory, count);
		}

		/// <summary>
		/// Sets the given favorite as selected.
		/// </summary>
		/// <param name="favorite"></param>
		private void SetSelected(Favorite favorite)
		{
			if (favorite == m_Selected)
				return;

			m_Selected = favorite;

			foreach (IFavoritesComponentPresenter presenter in m_ChildrenFactory)
				presenter.Selected = presenter.Favorite == m_Selected;

			RefreshDialButton();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sets the enabled state and label of the dial button.
		/// </summary>
		private void RefreshDialButton()
		{
			eConferenceSourceType type = m_Selected == null || Room == null
				                             ? eConferenceSourceType.Unknown
				                             : Room.ConferenceManager.DialingPlan.GetSourceType(m_Selected);
			string callTypeString = StringUtils.NiceName(type);

			GetView().SetCallTypeLabel(callTypeString);
			GetView().EnableDialButton(m_Selected != null);
		}

		/// <summary>
		/// Unsubscribes from all of the child presenters.
		/// </summary>
		private void UnsubscribeChildren()
		{
			foreach (IFavoritesComponentPresenter presenter in m_ChildrenFactory)
				Unsubscribe(presenter);
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Subscribes to the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Subscribe(IFavoritesComponentPresenter child)
		{
			if (child == null)
				return;

			child.OnPressed += ChildOnSelectedStateChanged;
			child.OnIsFavoriteStateChanged += ChildOnIsFavoriteStateChanged;
		}

		/// <summary>
		/// Unsubscribes from the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Unsubscribe(IFavoritesComponentPresenter child)
		{
			if (child == null)
				return;

			child.OnPressed -= ChildOnSelectedStateChanged;
			child.OnIsFavoriteStateChanged -= ChildOnIsFavoriteStateChanged;
		}

		/// <summary>
		/// Called when a child becomes selected or deselected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildOnSelectedStateChanged(object sender, EventArgs eventArgs)
		{
			IFavoritesComponentPresenter presenter = sender as IFavoritesComponentPresenter;
			if (presenter == null)
				return;

			Favorite favorite = presenter.Favorite;

			SetSelected(favorite == m_Selected ? null : favorite);
		}

		/// <summary>
		/// Called when a child becomes favorited or unfavorited.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildOnIsFavoriteStateChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IFavoritesView view)
		{
			base.Subscribe(view);

			view.OnDialButtonPressed += ViewOnDialButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IFavoritesView view)
		{
			base.Unsubscribe(view);

			view.OnDialButtonPressed -= ViewOnDialButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the dial button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDialButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_Selected != null)
				Dial(m_Selected);
			m_Selected = null;
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			m_Selected = null;

			base.ViewOnVisibilityChanged(sender, args);
		}

		#endregion
	}
}
