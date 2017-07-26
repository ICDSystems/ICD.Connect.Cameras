using System;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Requests
{
	public sealed class GetReservationsRequest : AbstractRequest<GetReservationsResult>
	{
		private const string ACTION = "GetReservations";

		private readonly DateTime m_Start;
		private readonly DateTime m_End;

		/// <summary>
		/// Gets the name of the service method.
		/// </summary>
		protected override string SoapAction { get { return ACTION; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public GetReservationsRequest(DateTime start, DateTime end)
		{
			m_Start = start;
			m_End = end;
		}

		/// <summary>
		/// Builds the resulting object from the xml response.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override GetReservationsResult ResultFromXml(string xml)
		{
			return GetReservationsResult.FromXml(xml);
		}

		/// <summary>
		/// Adds the parameters by calling addParam for each name/value pair.
		/// </summary>
		/// <param name="addParam"></param>
		protected override void AddSoapParams(Action<string, object> addParam)
		{
			addParam("Start", AsureUtils.DateTimeToString(m_Start));
			addParam("End", AsureUtils.DateTimeToString(m_End));
		}
	}
}
