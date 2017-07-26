using System;
using System.Linq;
using ICD.Common.Services.Logging;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Favorites;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public sealed class RecentCallPresenter : AbstractComponentPresenter<IRecentCallView>, IRecentCallPresenter
	{
		public event EventHandler<BoolEventArgs> OnSelectedStateChanged;

		private IConferenceSource m_Source;
		private bool m_Selected;
	    private readonly SafeCriticalSection m_SafeRefreshSection;

		#region Properties

		/// <summary>
		/// Gets/sets the property source.
		/// </summary>
		public IConferenceSource Source
		{
			get { return m_Source; }
			set
			{
				if (value == m_Source)
					return;

				Unsubscribe(m_Source);
				m_Source = value;
				Subscribe(m_Source);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets/sets the selected state of the source.
		/// </summary>
		public bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				OnSelectedStateChanged.Raise(this, new BoolEventArgs(m_Selected));

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public RecentCallPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
            m_SafeRefreshSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnSelectedStateChanged = null;

			base.Dispose();

			Unsubscribe(m_Source);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IRecentCallView view)
		{
            base.Refresh(view);
            m_SafeRefreshSection.Enter();
            try
		    {

		        string name = m_Source == null ? string.Empty : m_Source.Name;
		        if (string.IsNullOrEmpty(name))
		            name = m_Source == null ? string.Empty : m_Source.Number;

		        string details = GetDetailText();
		        bool favorite = GetIsFavorite();
		        eRecentCallIconMode icon = GetIcon();

		        view.SetName(name);
		        view.SetDetailsText(details);
		        view.SetFavorite(favorite);
		        view.SetUserIcon(icon);
		        view.SetSelected(m_Selected);
		    }
		    finally
		    {
		        m_SafeRefreshSection.Leave();
		    }
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns the detail text for the source.
		/// </summary>
		/// <returns></returns>
		private string GetDetailText()
		{
			if (m_Source == null)
				return string.Empty;

			string answer = StringUtils.NiceName(m_Source.AnswerState);
			string date = string.Format("{0:hh:mm (tt) yyyy-MM-dd}", m_Source.Start);

			return string.Format("{0} - {1}", answer, date);
		}

		/// <summary>
		/// Returns true if the source is a favorite.
		/// </summary>
		/// <returns></returns>
		private bool GetIsFavorite()
		{
			return m_Source != null &&
			       Room != null &&
			       Room.ConferenceManager.Favorites.GetFavoritesByContactNumber(m_Source.Number).FirstOrDefault() != null;
		}

		/// <summary>
		/// Gets the icon for the source.
		/// </summary>
		/// <returns></returns>
		private eRecentCallIconMode GetIcon()
		{
			eConferenceSourceType type = m_Source == null ? eConferenceSourceType.Unknown : m_Source.SourceType;

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

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IRecentCallView view)
		{
			base.Subscribe(view);

			view.OnFavoriteButtonPressed += ViewOnFavoriteButtonPressed;
			view.OnPressed += ViewOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IRecentCallView view)
		{
			base.Unsubscribe(view);

			view.OnFavoriteButtonPressed -= ViewOnFavoriteButtonPressed;
			view.OnPressed -= ViewOnPressed;
		}

		/// <summary>
		/// Called when the user presses the view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			Selected = !Selected;
		}

		/// <summary>
		/// Called when the user presses the favorite button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnFavoriteButtonPressed(object sender, EventArgs eventArgs)
		{
			IContact contact = GetContact();

			if (Room == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to toggle favorite - room is null");
				return;
			}

			if (GetIsFavorite())
				Room.ConferenceManager.Favorites.RemoveFavorite(contact);
			else
				Room.ConferenceManager.Favorites.SubmitFavorite(contact);

			RefreshIfVisible();
		}

		/// <summary>
		/// Gets the contact from source.
		/// </summary>
		/// <returns></returns>
		private IContact GetContact()
		{
			// Search for favorite by number
			Favorite favorite = Room == null ? null : Room.ConferenceManager.Favorites.GetFavoritesByContactNumber(m_Source.Number).FirstOrDefault();
			if (favorite != null)
				return favorite;

			// Create contact from the source
			return new Contact(m_Source.Name, new IContactMethod[] {new ContactMethod(m_Source.Number)});
		}

		#endregion

		#region Source Callbacks

		/// <summary>
		/// Subscribe to the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Subscribe(IConferenceSource source)
		{
			if (source == null)
				return;

			source.OnNameChanged += SourceOnNameChanged;
			source.OnSourceTypeChanged += SourceOnSourceTypeChanged;
		}

		/// <summary>
		/// Unsubscribe from the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Unsubscribe(IConferenceSource source)
		{
			if (source == null)
				return;

			source.OnNameChanged -= SourceOnNameChanged;
			source.OnSourceTypeChanged -= SourceOnSourceTypeChanged;
		}

		/// <summary>
		/// Called when the source type changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SourceOnSourceTypeChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the source name changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void SourceOnNameChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
