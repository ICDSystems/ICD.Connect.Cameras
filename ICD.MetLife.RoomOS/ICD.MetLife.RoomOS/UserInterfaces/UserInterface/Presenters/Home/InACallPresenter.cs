using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.Endpoints.Sources;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.ShareMenu;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Home
{
	/// <summary>
	/// InACallPresenter represents the background subpage that lists the active call sources.
	/// </summary>
	public sealed class InACallPresenter : AbstractPresenter<IInACallView>, IInACallPresenter
	{
		private readonly CallStatusComponentPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		private IRoomNumbersPresenter m_RoomNumbers;
		private IHomeMeetingsPresenter m_HomeMeetings;

		private IRoomNumbersPresenter RoomNumbers
		{
			get
			{
				return m_RoomNumbers ??
				       (m_RoomNumbers = Navigation.LazyLoadPresenter<IRoomNumbersPresenter>());
			}
		}

		private IHomeMeetingsPresenter HomeMeetings
		{
			get
			{
				if (m_HomeMeetings != null)
					return m_HomeMeetings;

				m_HomeMeetings = Navigation.LazyLoadPresenter<IHomeMeetingsPresenter>();
				m_HomeMeetings.OnHasVisibleMeetingsChanged += HomeMeetingsOnHasVisibleMeetingsChanged;

				return m_HomeMeetings;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public InACallPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_ChildrenFactory = new CallStatusComponentPresenterFactory(nav, ItemFactory);
			m_RefreshSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IInACallView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IConferenceSource[] sources = GetSources().ToArray();
				foreach (ICallStatusPresenter presenter in m_ChildrenFactory.BuildChildren(sources))
					presenter.ShowView(true);

				RoomNumbers.ShowView(true);
				HomeMeetings.ShowView(CanShowMeetings());
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns the current active sources.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConferenceSource> GetSources()
		{
			IConference conference = Room == null ? null : Room.ConferenceManager.ActiveConference;
			return conference == null ? Enumerable.Empty<IConferenceSource>() : conference.GetSources().Where(s => s.GetIsOnline());
		}

		/// <summary>
		/// Returns true if we are not currently in a conference, and there are scheduled meetings.
		/// </summary>
		/// <returns></returns>
		private bool CanShowMeetings()
		{
			return Room != null && !Room.ConferenceManager.IsInCall && HomeMeetings.HasVisibleMeetings;
		}

		/// <summary>
		/// Generates the given number of views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<ICallStatusView> ItemFactory(ushort count)
		{
			return GetView().GetChildCallViews(ViewFactory, count);
		}

		/// <summary>
		/// Called when meetings become available/unavailable on the HomeMeetings presenter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void HomeMeetingsOnHasVisibleMeetingsChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(MetlifeRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnShutdown += RoomOnShutdown;
			room.Routing.OnSourceAutoRouted += RoomOnSourceAutoRouted;

			room.ConferenceManager.OnInCallChanged += ConferenceManagerOnInCallChanged;
			room.ConferenceManager.OnRecentSourceAdded += ConferenceManagerOnRecentSourceAdded;
			room.ConferenceManager.OnActiveSourceStatusChanged += ConferenceManagerOnActiveSourceStatusChanged;
			room.ConferenceManager.OnActiveConferenceChanged += ConferenceManagerOnActiveConferenceChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnShutdown -= RoomOnShutdown;
			room.Routing.OnSourceAutoRouted -= RoomOnSourceAutoRouted;

			room.ConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;
			room.ConferenceManager.OnRecentSourceAdded -= ConferenceManagerOnRecentSourceAdded;
			room.ConferenceManager.OnActiveSourceStatusChanged -= ConferenceManagerOnActiveSourceStatusChanged;
			room.ConferenceManager.OnActiveConferenceChanged -= ConferenceManagerOnActiveConferenceChanged;
		}

		/// <summary>
		/// Called when the room shuts down.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnShutdown(object sender, EventArgs eventArgs)
		{
			ShowView(true);
		}

		/// <summary>
		/// When the room automatically routes a source and we're on the homepage, navigate to the share menu.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnSourceAutoRouted(object sender, SourceEventArgs eventArgs)
		{
			MetlifeSource source = eventArgs.Data as MetlifeSource;

			if (source != null && source.SourceFlags.HasFlag(eSourceFlags.Share))
				Navigation.NavigateTo<IShareMenuPresenter>();
		}

		/// <summary>
		/// Called when we enter/leave a call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ConferenceManagerOnInCallChanged(object sender, BoolEventArgs boolEventArgs)
		{
			// Show the view when we enter a call.
			if (boolEventArgs.Data)
				ShowView(true);
		}

		/// <summary>
		/// Called when an active source status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnActiveSourceStatusChanged(object sender, ConferenceSourceStatusEventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when a new source is added.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnRecentSourceAdded(object sender, ConferenceSourceEventArgs args)
		{
			ShowView(true);

			if (args.Data.GetIsOnline())
				RefreshIfVisible();
		}

		/// <summary>
		/// Called when the active conference changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="conferenceEventArgs"></param>
		private void ConferenceManagerOnActiveConferenceChanged(object sender, ConferenceEventArgs conferenceEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			// When the home page is visible we should always show the main nav.
			if (args.Data)
				Navigation.NavigateTo<IMainNavPresenter>();
			else
			{
				RoomNumbers.ShowView(false);
				HomeMeetings.ShowView(false);
			}
		}
	}
}
