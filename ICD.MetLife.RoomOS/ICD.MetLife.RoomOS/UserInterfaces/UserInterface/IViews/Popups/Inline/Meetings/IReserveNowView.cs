using System;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Meetings
{
	public interface IReserveNowView : IView
	{
		event EventHandler OnReserveButtonPressed;
		event EventHandler OnCancelButtonPressed;
		event EventHandler<DateTimeEventArgs> OnSelectedTimeChanged;

		/// <summary>
		/// Set the label that displays when the next meeting will be
		/// </summary>
		/// <param name="text">time of next meeting</param>
		void SetNextMeetingTime(string text);

		/// <summary>
		/// Set the enabled state of the "reserve" button
		/// </summary>
		/// <param name="enabled"></param>
		void SetReserveButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the hours, minutes and AM/PM currently displaye
		/// </summary>
		/// <param name="time"></param>
		void SetSelectedTime(DateTime time);
	}
}
