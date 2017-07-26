using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Meetings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Home
{
	public sealed partial class HomeMeetingsView : AbstractView, IHomeMeetingsView
	{
		private const string DATETIME_FORMAT = "h:mm tt";

		public event EventHandler OnCheckInButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public HomeMeetingsView(ISigInputOutput panel)
			: base(panel)
		{
		}

		public void SetCheckInButtonEnabled(bool enabled)
		{
			m_CheckInButton.Enable(enabled);
		}

		public void SetCheckInButtonMode(eCheckInMode mode)
		{
			m_CheckInButton.SetMode((ushort)mode);
		}

		public void SetCurrentMeeting(MeetingInfo meeting)
		{
			string text = MeetingInfoToString(meeting);
			m_CurrentMeetingText.SetLabelTextAtJoin(m_CurrentMeetingText.SerialLabelJoins.First(), text);
		}

		public void SetCurrentMeetingVisible(bool visible)
		{
			m_CurrentMeetingText.Show(visible);
		}

		public void SetNextMeeting(MeetingInfo meeting)
		{
			string text = MeetingInfoToString(meeting);
			m_NextMeetingText.SetLabelTextAtJoin(m_NextMeetingText.SerialLabelJoins.First(), text);
		}

		public void SetNextMeetingVisible(bool visible)
		{
			m_NextMeetingText.Show(visible);
		}

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CheckInButton.OnPressed += CheckInButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CheckInButton.OnPressed -= CheckInButtonOnPressed;
		}

		private void CheckInButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCheckInButtonPressed.Raise(this);
		}

		/// <summary>
		/// Returns the HTML for a single label representing a MeetingInfo.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		private static string MeetingInfoToString(MeetingInfo info)
		{
			string start = info.StartTime == null ? null : ((DateTime)info.StartTime).ToString(DATETIME_FORMAT);
			string end = info.EndTime == null ? null : ((DateTime)info.EndTime).ToString(DATETIME_FORMAT);

			return string.Format("{0} - {1}{2}{3}{2}{4}", start, end, HtmlUtils.NEWLINE, info.MeetingName, info.OrganizerName);
		}
	}
}
