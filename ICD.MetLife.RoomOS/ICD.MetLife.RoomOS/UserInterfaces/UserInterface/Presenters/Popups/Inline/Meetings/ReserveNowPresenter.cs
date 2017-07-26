using System;
using ICD.Common.Services.Logging;
using ICD.Connect.Rooms;
using ICD.Connect.Scheduling.Asure;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Model;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Meetings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Meetings
{
	public sealed class ReserveNowPresenter : AbstractPopupPresenter<IReserveNowView>, IReserveNowPresenter
	{
		private AsureDevice m_Asure;
		private ReservationData m_NextReservation;
		private DateTime m_SelectedEndTime;

		/// <summary>
		/// Title for the popup
		/// </summary>
		protected override string Title { get { return "Reserve Now"; } }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public ReserveNowPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		#region Methods

		protected override void Refresh(IReserveNowView view)
		{
			if (m_Asure != null && m_Asure.GetReservationInProgress())
			{
				ShowView(false);
				return;
			}

			m_NextReservation = m_Asure == null ? null : m_Asure.GetNextReservation();
			DateTime nextReservationTime = m_NextReservation == null
				                               ? DateTime.MaxValue
				                               : m_NextReservation.ScheduleData.Start ?? DateTime.MaxValue;

			// Set the meeting time label
			if (m_NextReservation == null)
				view.SetNextMeetingTime("for the rest of the day"); //TODO: Add Localization
			else
			{
				view.SetNextMeetingTime(string.Format("until {0} ({1})",
				                                      nextReservationTime.ToShortTimeString(),
				                                      (nextReservationTime - IcdEnvironment.GetLocalTime()).ToReadableString()));
			}

			DateTime localSelectedTime = SelectedDateTimeToLocalDateTime(m_SelectedEndTime);

			// Is valid if there are at least 10 minutes for the meeting
			bool isValidEndTime = (localSelectedTime - IcdEnvironment.GetLocalTime()).TotalMinutes > 10;

			// Is invalid if time is before now
			isValidEndTime &= localSelectedTime > IcdEnvironment.GetLocalTime();

			// Is invalid if the time is after the next meeting starts
			isValidEndTime &= localSelectedTime < nextReservationTime;

			view.SetReserveButtonEnabled(isValidEndTime);
			view.SetSelectedTime(m_SelectedEndTime);
		}

		private static DateTime SelectedDateTimeToLocalDateTime(DateTime time)
		{
			DateTime local = IcdEnvironment.GetLocalTime();

			return new DateTime(local.Year,
			                    local.Month,
			                    local.Day,
			                    time.Hour,
			                    time.Minute,
			                    0);
		}

		#endregion

		#region Room Callbacks

		protected override void Subscribe(MetlifeRoom room)
		{
			base.Subscribe(room);

			m_Asure = room == null ? null : room.GetDevice<AsureDevice>();
			if (m_Asure != null)
				m_Asure.OnCacheUpdated += AsureOnCacheUpdated;
		}

		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (m_Asure != null)
				m_Asure.OnCacheUpdated -= AsureOnCacheUpdated;
		}

		private void AsureOnCacheUpdated(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IReserveNowView view)
		{
			base.Subscribe(view);

			view.OnSelectedTimeChanged += ViewOnSelectedTimeChanged;
			view.OnReserveButtonPressed += ViewOnOnReserveButtonPressed;
			view.OnCancelButtonPressed += ViewOnOnCancelButtonPressed;
		}

		protected override void Unsubscribe(IReserveNowView view)
		{
			base.Unsubscribe(view);

			view.OnSelectedTimeChanged -= ViewOnSelectedTimeChanged;
			view.OnReserveButtonPressed -= ViewOnOnReserveButtonPressed;
			view.OnCancelButtonPressed -= ViewOnOnCancelButtonPressed;
		}

		private void ViewOnOnCancelButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		private void ViewOnOnReserveButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_Asure == null)
				return;

			try
			{
				m_Asure.SubmitReservation("In-Room Reservation", "", IcdEnvironment.GetLocalTime(), m_SelectedEndTime);
				ShowView(false);
			}
			catch (Exception e)
			{
				Logger.AddEntry(eSeverity.Error, "Failed make reservation - {0}", e.Message);
				Navigation.NavigateTo<IAlertBoxPresenter>().Enqueue("Failed to Reserve", e.Message, new AlertOption("Close"));
			}
		}

		private void ViewOnSelectedTimeChanged(object sender, DateTimeEventArgs args)
		{
			m_SelectedEndTime = args.Data;
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			if (args.Data)
				m_SelectedEndTime = IcdEnvironment.GetLocalTime();

			base.ViewOnVisibilityChanged(sender, args);
		}

		#endregion
	}
}
