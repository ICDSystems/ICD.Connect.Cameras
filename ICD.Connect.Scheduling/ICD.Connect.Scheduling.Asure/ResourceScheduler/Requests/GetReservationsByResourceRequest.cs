using System;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Requests
{
	public sealed class GetReservationsByResourceRequest : AbstractRequest<GetReservationsByResourceResult>
	{
		private const string ACTION = "GetReservationsByResource";

		private readonly DateTime m_Start;
		private readonly DateTime m_End;
		private readonly int m_ResourceId;

		/// <summary>
		/// Gets the name of the service method.
		/// </summary>
		protected override string SoapAction { get { return ACTION; } }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="resourceId"></param>
		public GetReservationsByResourceRequest(DateTime start, DateTime end, int resourceId)
		{
			m_Start = start;
			m_End = end;
			m_ResourceId = resourceId;
		}

		/// <summary>
		/// Builds the resulting object from the xml response.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override GetReservationsByResourceResult ResultFromXml(string xml)
		{
			return GetReservationsByResourceResult.FromXml(xml);
		}

		/// <summary>
		/// Adds the parameters by calling addParam for each name/value pair.
		/// </summary>
		/// <param name="addParam"></param>
		protected override void AddSoapParams(Action<string, object> addParam)
		{
			addParam("Start", AsureUtils.DateTimeToString(m_Start));
			addParam("End", AsureUtils.DateTimeToString(m_End));
			addParam("ResourceId", m_ResourceId);
		}
	}
}
