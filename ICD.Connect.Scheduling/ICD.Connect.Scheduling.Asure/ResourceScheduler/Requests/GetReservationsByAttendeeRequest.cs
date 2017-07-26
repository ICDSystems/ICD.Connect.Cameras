using System;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Requests
{
	public sealed class GetReservationsByAttendeeRequest : AbstractRequest<GetReservationsByAttendeeResult>
	{
		private const string ACTION = "GetReservationsByAttendee";

		private readonly DateTime m_Start;
		private readonly DateTime m_End;
		private readonly int m_AttendeeId;

		/// <summary>
		/// Gets the name of the service method.
		/// </summary>
		protected override string SoapAction { get { return ACTION; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="attendeeId"></param>
		public GetReservationsByAttendeeRequest(DateTime start, DateTime end, int attendeeId)
		{
			m_Start = start;
			m_End = end;
			m_AttendeeId = attendeeId;
		}

		/// <summary>
		/// Adds the parameters by calling addParam for each name/value pair.
		/// </summary>
		/// <param name="addParam"></param>
		protected override void AddSoapParams(Action<string, object> addParam)
		{
			addParam("Start", AsureUtils.DateTimeToString(m_Start));
			addParam("End", AsureUtils.DateTimeToString(m_End));
			addParam("AttendeeId", m_AttendeeId);
		}

		/// <summary>
		/// Builds the resulting object from the xml response.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override GetReservationsByAttendeeResult ResultFromXml(string xml)
		{
			return GetReservationsByAttendeeResult.FromXml(xml);
		}
	}
}
