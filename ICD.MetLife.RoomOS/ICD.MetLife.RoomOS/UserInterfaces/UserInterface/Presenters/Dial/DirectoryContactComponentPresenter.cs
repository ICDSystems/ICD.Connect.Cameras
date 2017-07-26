using System;
using ICD.Common.Services.Logging;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Favorites;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public sealed class DirectoryContactComponentPresenter : AbstractDirectoryComponentPresenter,
	                                                         IDirectoryContactComponentPresenter
	{
		private IContact m_Contact;

		/// <summary>
		/// Gets/sets the contact.
		/// </summary>
		public IContact Contact
		{
			get { return m_Contact; }
			set
			{
				if (value == m_Contact)
					return;

				m_Contact = value;

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
		public DirectoryContactComponentPresenter(int room, INavigationController nav, IViewFactory views,
		                                          ICore core)
			: base(room, nav, views, core)
		{
		}

		#region Protected Methods

		/// <summary>
		/// Returns true if the component is favorited.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsFavorite()
		{
			return m_Contact != null && Room != null && Room.ConferenceManager.Favorites.ContainsFavorite(m_Contact);
		}

		/// <summary>
		/// Returns the icon for the component.
		/// </summary>
		/// <returns></returns>
		protected override eRecentCallIconMode GetIcon()
		{
			return eRecentCallIconMode.User;
		}

		/// <summary>
		/// Returns true if the favorite button should be visible.
		/// </summary>
		/// <returns></returns>
		protected override bool GetFavoriteButtonVisible()
		{
			//return true;
			return false;
		}

		/// <summary>
		/// Returns the name for the component.
		/// </summary>
		/// <returns></returns>
		protected override string GetName()
		{
			return m_Contact == null ? string.Empty : m_Contact.Name;
		}

		/// <summary>
		/// Called when the user presses the favorite button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnFavoriteButtonPressed(object sender, EventArgs eventArgs)
		{
			base.ViewOnFavoriteButtonPressed(sender, eventArgs);

			if (m_Contact == null)
				return;

			if (Room == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to toggle favorite - room is null");
				return;
			}

			if (GetIsFavorite())
				Room.ConferenceManager.Favorites.RemoveFavorite(m_Contact);
			else
				Room.ConferenceManager.Favorites.SubmitFavorite(m_Contact);

			RefreshIfVisible();
		}

		#endregion
	}
}
