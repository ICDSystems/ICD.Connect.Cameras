using System;
using System.Collections.Generic;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Meetings
{
	public interface IMeetingsView : IView
	{
		event EventHandler OnReserveNowButtonPressed;
		event EventHandler OnReserveFutureButtonPressed;
		event EventHandler OnCheckInButtonPressed;

		/// <summary>
		/// Sets the meeting labels count and text.
		/// </summary>
		/// <param name="meetings"></param>
		void SetMeetingLabels(IEnumerable<MeetingInfo> meetings);

		/// <summary>
		/// Sets the visibility of the "no meetings" label.
		/// </summary>
		/// <param name="visibility"></param>
		void SetNoMeetingsLabelVisibility(bool visibility);

		/// <summary>
		/// Sets the visibility of the meetings button list.
		/// </summary>
		/// <param name="visibility"></param>
		void SetMeetingsButtonListVisibility(bool visibility);

		/// <summary>
		/// Sets the enabled state of the "reserve now" button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetReserveNowButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the "reserve future" button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetReserveFutureButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the "check in" button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetCheckInButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the selected state of the "check in" button.
		/// </summary>
		/// <param name="selected"></param>
		void SetCheckInButtonSelected(bool selected);

		void SetCheckInButtonMode(eCheckInMode mode);
	}

	public enum eCheckInMode
	{
		NoMeeting = 0,
		CheckIn = 1,
		CheckOut = 2,
		CheckInNotAvailable = 3,
		CheckingIn = 4,
		CheckingOut = 5,
		CheckOutNotAvailable = 6
	}
}
