using System;
using ICD.Common.Services.Logging;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.Favorites;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public sealed class FavoritesComponentPresenter : AbstractFavoritesAndDirectoryComponentPresenter,
	                                                  IFavoritesComponentPresenter
	{
		public event EventHandler OnIsFavoriteStateChanged;

		private Favorite m_Favorite;

		/// <summary>
		/// Gets/sets the favorite.
		/// </summary>
		public Favorite Favorite
		{
			get { return m_Favorite; }
			set
			{
				if (value == m_Favorite)
					return;

				m_Favorite = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public FavoritesComponentPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Returns true if the component is favorited.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsFavorite()
		{
			return true;
		}

		/// <summary>
		/// Returns the icon for the component.
		/// </summary>
		/// <returns></returns>
		protected override eRecentCallIconMode GetIcon()
		{
			eConferenceSourceType type = m_Favorite == null || Room == null
				                             ? eConferenceSourceType.Unknown
				                             : Room.ConferenceManager.DialingPlan.GetSourceType(m_Favorite);

			switch (type)
			{
				case eConferenceSourceType.Unknown:
					return eRecentCallIconMode.Audio;
				case eConferenceSourceType.Audio:
					return eRecentCallIconMode.Audio;
				case eConferenceSourceType.Video:
					return eRecentCallIconMode.Video;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Returns true if the favorite button should be visible.
		/// </summary>
		/// <returns></returns>
		protected override bool GetFavoriteButtonVisible()
		{
			return true;
		}

		/// <summary>
		/// Returns the name for the component.
		/// </summary>
		/// <returns></returns>
		protected override string GetName()
		{
			return m_Favorite == null ? string.Empty : m_Favorite.Name;
		}

		/// <summary>
		/// Called when the user presses the favorite button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnFavoriteButtonPressed(object sender, EventArgs eventArgs)
		{
			base.ViewOnFavoriteButtonPressed(sender, eventArgs);

			if (Room == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to toggle favorite - no room");
				return;
			}

			Room.ConferenceManager.Favorites.RemoveFavorite(Favorite);
			OnIsFavoriteStateChanged.Raise(this);
		}
	}
}
