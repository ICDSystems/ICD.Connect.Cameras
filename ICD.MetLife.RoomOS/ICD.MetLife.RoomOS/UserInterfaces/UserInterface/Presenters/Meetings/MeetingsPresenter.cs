using System;
using System.Linq;
using ICD.Connect.Rooms;
using ICD.Connect.Scheduling.Asure;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Model;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Meetings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Meetings
{
	public sealed class MeetingsPresenter : AbstractMainPresenter<IMeetingsView>, IMeetingsPresenter
	{
		// Refresh the page every minute
		private const long REFRESH_PERIOD = 1 * 60 * 1000;

		private readonly SafeTimer m_RefreshTimer;

		private ReservationData[] m_Reservations;
		private AsureDevice m_Asure;

		private eCheckInMode? m_CachedCheckInMode;

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Meetings"; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public MeetingsPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_Reservations = new ReservationData[0];

			m_RefreshTimer = new SafeTimer(RefreshIfVisible, REFRESH_PERIOD, REFRESH_PERIOD);
		}

		#region Methods

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
		protected override void Refresh(IMeetingsView view)
		{
			base.Refresh(view);

			m_Reservations = m_Asure == null ? new ReservationData[0] : m_Asure.GetReservations().ToArray();

			// Build the meetings list
			view.SetMeetingLabels(m_Reservations.Select<ReservationData, MeetingInfo>(GetMeetingInfo));

			// Toggle button states
			bool noMeetingsVisibility = m_Reservations.Length == 0;
			bool meetingsListVisibility = m_Reservations.Length > 0;
			bool reserveNowEnabled = m_Asure != null && !m_Asure.GetReservationInProgress();
			const bool reserveFutureEnabled = false; // todo - need a subpage
			bool checkedIn = m_Asure != null && m_Asure.GetCheckedInState();

			// don't change button state if already checking in or out
			if (m_CachedCheckInMode != eCheckInMode.CheckingIn && m_CachedCheckInMode != eCheckInMode.CheckingOut)
			{
				if (noMeetingsVisibility || m_Asure == null)
					m_CachedCheckInMode = eCheckInMode.NoMeeting;
				else if (!m_Asure.GetCheckedInState())
					m_CachedCheckInMode = m_Asure.CanCheckIn() ? eCheckInMode.CheckIn : eCheckInMode.CheckInNotAvailable;
				else
					m_CachedCheckInMode = m_Asure.CanCheckOut() ? eCheckInMode.CheckOut : eCheckInMode.CheckOutNotAvailable;
			}

			bool checkInEnabled = (m_CachedCheckInMode == eCheckInMode.CheckIn ||
			                       m_CachedCheckInMode == eCheckInMode.CheckOut);

			view.SetNoMeetingsLabelVisibility(noMeetingsVisibility);
			view.SetMeetingsButtonListVisibility(meetingsListVisibility);
			view.SetReserveNowButtonEnabled(reserveNowEnabled);
			view.SetReserveFutureButtonEnabled(reserveFutureEnabled);
			view.SetCheckInButtonEnabled(checkInEnabled);
			view.SetCheckInButtonSelected(checkedIn);
			view.SetCheckInButtonMode(m_CachedCheckInMode.Value);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the reservation as a meeting info struct.
		/// </summary>
		/// <param name="reservation"></param>
		/// <returns></returns>
		private static MeetingInfo GetMeetingInfo(ReservationData reservation)
		{
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
		protected override void Subscribe(IMeetingsView view)
		{
			base.Subscribe(view);

			view.OnCheckInButtonPressed += ViewOnCheckInButtonPressed;
			view.OnReserveFutureButtonPressed += ViewOnReserveFutureButtonPressed;
			view.OnReserveNowButtonPressed += ViewOnReserveNowButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IMeetingsView view)
		{
			base.Unsubscribe(view);

			view.OnCheckInButtonPressed -= ViewOnCheckInButtonPressed;
			view.OnReserveFutureButtonPressed -= ViewOnReserveFutureButtonPressed;
			view.OnReserveNowButtonPressed -= ViewOnReserveNowButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the "reserve now" button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnReserveNowButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.LazyLoadPresenter<IReserveNowPresenter>().ShowView(true);
		}

		/// <summary>
		/// Called when the user presses the "reserve future" button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnReserveFutureButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Called when the user presses the "check in" button.
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
			if (m_Asure == null)
				throw new InvalidOperationException("Asure device is null");

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
			if (m_Asure == null)
				throw new InvalidOperationException("Asure device is null");

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

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			// Ignore hide
			if (!args.Data)
				return;

			if (m_Asure != null)
				m_Asure.RefreshCacheAsync();
		}

		#endregion
	}
}
