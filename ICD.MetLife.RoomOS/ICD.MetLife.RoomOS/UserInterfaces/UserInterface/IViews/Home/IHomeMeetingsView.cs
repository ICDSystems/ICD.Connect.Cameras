using System;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Meetings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home
{
	public interface IHomeMeetingsView : IView
	{
		event EventHandler OnCheckInButtonPressed;

		void SetCheckInButtonEnabled(bool enabled);

		void SetCheckInButtonMode(eCheckInMode mode);

		void SetCurrentMeeting(MeetingInfo meeting);

		void SetCurrentMeetingVisible(bool visible);

		void SetNextMeeting(MeetingInfo meeting);

		void SetNextMeetingVisible(bool visible);
	}
}
