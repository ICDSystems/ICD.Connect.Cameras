using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Meetings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Meetings
{
	public sealed partial class MeetingsView : AbstractView, IMeetingsView
	{
		private const string MEETING_FORMAT =
			@"<FONT size=""24"" face=""Crestron Unicode"" color=""#fefdfd"">{0} - {1}<BR>" +
			@"{2}<BR><FONT size=""20"">{3}</FONT></FONT>";

		private const string DATETIME_FORMAT = "h:mm tt";

		public event EventHandler OnReserveNowButtonPressed;
		public event EventHandler OnReserveFutureButtonPressed;
		public event EventHandler OnCheckInButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public MeetingsView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Sets the meeting labels count and text.
		/// </summary>
		/// <param name="meetings"></param>
		public void SetMeetingLabels(IEnumerable<MeetingInfo> meetings)
		{
			string[] labelsArray = meetings.Take(m_MeetingButtonList.MaxSize)
			                               .Select<MeetingInfo, string>(MeetingInfoToString)
			                               .ToArray();

			m_MeetingButtonList.SetNumberOfItems((ushort)labelsArray.Length);

			for (ushort index = 0; index < (ushort)labelsArray.Length; index++)
			{
				m_MeetingButtonList.SetItemVisible(index, true);
				m_MeetingButtonList.SetItemLabel(index, labelsArray[index]);
			}
		}

		/// <summary>
		/// Sets the visibility of the "no meetings" label.
		/// </summary>
		/// <param name="visibility"></param>
		public void SetNoMeetingsLabelVisibility(bool visibility)
		{
			m_NoMeetingsLabel.Show(visibility);
		}

		/// <summary>
		/// Sets the visibility of the meetings button list.
		/// </summary>
		/// <param name="visibility"></param>
		public void SetMeetingsButtonListVisibility(bool visibility)
		{
			m_MeetingButtonList.Show(visibility);
		}

		/// <summary>
		/// Sets the enabled state of the "reserve now" button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetReserveNowButtonEnabled(bool enabled)
		{
			m_ReserveNowButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the "reserve future" button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetReserveFutureButtonEnabled(bool enabled)
		{
			m_ReserveFutureButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the "check in" button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetCheckInButtonEnabled(bool enabled)
		{
			m_CheckInButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the selected state of the "check in" button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetCheckInButtonSelected(bool selected)
		{
			m_CheckInButton.SetSelected(selected);
		}

		public void SetCheckInButtonMode(eCheckInMode mode)
		{
			m_CheckInButton.SetMode((ushort)mode);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ReserveNowButton.OnPressed += ReserveNowButtonOnPressed;
			m_ReserveFutureButton.OnPressed += ReserveFutureButtonOnPressed;
			m_CheckInButton.OnPressed += CheckInButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ReserveNowButton.OnPressed -= ReserveNowButtonOnPressed;
			m_ReserveFutureButton.OnPressed -= ReserveFutureButtonOnPressed;
			m_CheckInButton.OnPressed -= CheckInButtonOnPressed;
		}

		private void CheckInButtonOnPressed(object sender, EventArgs args)
		{
			OnCheckInButtonPressed.Raise(this);
		}

		private void ReserveFutureButtonOnPressed(object sender, EventArgs args)
		{
			OnReserveFutureButtonPressed.Raise(this);
		}

		private void ReserveNowButtonOnPressed(object sender, EventArgs args)
		{
			OnReserveNowButtonPressed.Raise(this);
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

			return string.Format(MEETING_FORMAT, start, end, info.MeetingName, info.OrganizerName);
		}

		#endregion
	}
}
