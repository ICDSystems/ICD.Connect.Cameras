using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Meetings
{
	public struct MeetingInfo
	{
		private readonly string m_MeetingName;
		private readonly string m_OrganizerName;
		private readonly DateTime? m_StartTime;
		private readonly DateTime? m_EndTime;

		public string MeetingName { get { return m_MeetingName; } }
		public string OrganizerName { get { return m_OrganizerName; } }
		public DateTime? StartTime { get { return m_StartTime; } }
		public DateTime? EndTime { get { return m_EndTime; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="meetingName"></param>
		/// <param name="organizerName"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		public MeetingInfo(string meetingName, string organizerName, DateTime? startTime, DateTime? endTime)
		{
			m_MeetingName = meetingName;
			m_OrganizerName = organizerName;
			m_StartTime = startTime;
			m_EndTime = endTime;
		}
	}
}
