using System;
using System.Linq;
using ICD.Connect.Rooms;
using ICD.Connect.Settings.Core;
using ICD.Connect.Scheduling.Asure;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Model;
using ICD.Common.Properties;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Meetings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Home
{
	public sealed class HomeMeetingsPresenter : AbstractPresenter<IHomeMeetingsView>, IHomeMeetingsPresenter
	{
		// Refresh the page every minute
		private const long REFRESH_PERIOD = 1 * 60 * 1000;

		/// <summary>
		/// Raised when a meeting is added, or all meetings are removed from the subpage.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnHasVisibleMeetingsChanged;

		private readonly SafeTimer m_RefreshTimer;
		private AsureDevice m_Asure;
		private bool m_HasVisibleMeetings;

		private eCheckInMode? m_CachedCheckInMode;

		/// <summary>
		/// Returns true if there are meetings currently being displayed.
		/// </summary>
		public bool HasVisibleMeetings { get { return GetCurrentReservation() != null || GetNextReservation() != null; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public HomeMeetingsPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_RefreshTimer = new SafeTimer(RefreshIfVisible, REFRESH_PERIOD, REFRESH_PERIOD);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_RefreshTimer.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IHomeMeetingsView view)
		{
			base.Refresh(view);

			ReservationData currentReservation = GetCurrentReservation();
			ReservationData nextReservation = GetNextReservation();

			MeetingInfo current = GetMeetingInfo(currentReservation);
			bool currentVisible = currentReservation != null;
			MeetingInfo next = GetMeetingInfo(nextReservation);
			bool nextVisible = nextReservation != null;

			// don't change button state if already checking in or out
			if (m_CachedCheckInMode != eCheckInMode.CheckingIn && m_CachedCheckInMode != eCheckInMode.CheckingOut)
			{
				if (!currentVisible || m_Asure == null)
					m_CachedCheckInMode = eCheckInMode.NoMeeting;
				else if (!m_Asure.GetCheckedInState())
					m_CachedCheckInMode = m_Asure.CanCheckIn() ? eCheckInMode.CheckIn : eCheckInMode.CheckInNotAvailable;
				else
					m_CachedCheckInMode = m_Asure.CanCheckOut() ? eCheckInMode.CheckOut : eCheckInMode.CheckOutNotAvailable;
			}
			bool startButtonEnabled = (m_CachedCheckInMode == eCheckInMode.CheckIn ||
			                           m_CachedCheckInMode == eCheckInMode.CheckOut);

			view.SetCurrentMeeting(current);
			view.SetCurrentMeetingVisible(currentVisible);
			view.SetNextMeeting(next);
			view.SetNextMeetingVisible(nextVisible);
			view.SetCheckInButtonEnabled(startButtonEnabled);
			view.SetCheckInButtonMode(m_CachedCheckInMode.Value);

			bool visibleMeetings = currentReservation != null || nextReservation != null;
			if (visibleMeetings == m_HasVisibleMeetings)
				return;

			m_HasVisibleMeetings = true;
			OnHasVisibleMeetingsChanged.Raise(this, new BoolEventArgs(m_HasVisibleMeetings));
		}

		#region Private Methods

		/// <summary>
		/// Gets the reservation in progress.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private ReservationData GetCurrentReservation()
		{
			return m_Asure == null ? null : m_Asure.GetCurrentReservation();
		}

		/// <summary>
		/// Gets the next scheduled reservation that is not currently in progress.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		private ReservationData GetNextReservation()
		{
			return m_Asure == null ? null : m_Asure.GetNextReservation();
		}

		/// <summary>
		/// Gets the reservation as a meeting info struct.
		/// </summary>
		/// <param name="reservation"></param>
		/// <returns></returns>
		private static MeetingInfo GetMeetingInfo(ReservationData reservation)
		{
			if (reservation == null)
				return default(MeetingInfo);

			string name = reservation.ReservationBaseData.Description;
			ReservationAttendeeData attendee = reservation.ReservationBaseData
			                                              .ReservationAttendees
			                                              .FirstOrDefault();
			string attendeeName = attendee == null ? string.Empty : attendee.FullName;
			DateTime? start = reservation.ScheduleData.Start;
			DateTime? end = reservation.ScheduleData.End;

			return new MeetingInfo(name, attendeeName, start, end);
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

			m_Asure = room == null ? null : room.GetDevice<AsureDevice>();
			if (m_Asure != null)
				m_Asure.OnCacheUpdated += ResourceSchedulerOnCacheUpdated;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (m_Asure != null)
				m_Asure.OnCacheUpdated -= ResourceSchedulerOnCacheUpdated;
		}

		/// <summary>
		/// Called when the resource scheduler cache becomes invalid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ResourceSchedulerOnCacheUpdated(object sender, EventArgs eventArgs)
		{
			m_CachedCheckInMode = null;
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IHomeMeetingsView view)
		{
			base.Subscribe(view);

			view.OnCheckInButtonPressed += ViewOnCheckInButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IHomeMeetingsView view)
		{
			base.Unsubscribe(view);

			view.OnCheckInButtonPressed -= ViewOnCheckInButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the checkin button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCheckInButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (m_CachedCheckInMode)
			{
				case eCheckInMode.CheckIn:
					m_CachedCheckInMode = eCheckInMode.CheckingIn;
					RefreshIfVisible();
					CheckIn();
					break;
				case eCheckInMode.CheckOut:
					m_CachedCheckInMode = eCheckInMode.CheckingOut;
					RefreshIfVisible();
					CheckOut();
					break;
			}
		}

		private void CheckIn()
		{
			ReservationData reservation = m_Asure.GetCurrentReservation();
			if (reservation == null)
				return;

			try
			{
				m_Asure.CheckIn(reservation.ReservationBaseData.Id);
			}
			catch (Exception e)
			{
				Navigation.NavigateTo<IAlertBoxPresenter>().Enqueue("Failed to Check In", e.Message, new AlertOption("Close"));
			}
		}

		private void CheckOut()
		{
			ReservationData reservation = m_Asure.GetCurrentReservation();
			if (reservation == null)
				return;

			try
			{
				m_Asure.CheckOut(reservation.ReservationBaseData.Id);
			}
			catch (Exception e)
			{
				Navigation.NavigateTo<IAlertBoxPresenter>().Enqueue("Failed to Check Out", e.Message, new AlertOption("Close"));
			}
		}

		#endregion
	}
}
