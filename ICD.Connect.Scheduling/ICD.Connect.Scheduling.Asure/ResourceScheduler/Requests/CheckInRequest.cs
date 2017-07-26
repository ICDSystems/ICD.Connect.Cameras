using System;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Requests
{
	public sealed class CheckInRequest : AbstractRequest<CheckInResult>
	{
		private const string ACTION = "CheckIn";

		private readonly int m_ReservationId;

		/// <summary>
		/// Gets the name of the service method.
		/// </summary>
		protected override string SoapAction { get { return ACTION; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reservationId"></param>
		public CheckInRequest(int reservationId)
		{
			m_ReservationId = reservationId;
		}

		/// <summary>
		/// Builds the resulting object from the xml response.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override CheckInResult ResultFromXml(string xml)
		{
			return CheckInResult.FromXml(xml);
		}

		/// <summary>
		/// Adds the parameters by calling addParam for each name/value pair.
		/// </summary>
		/// <param name="addParam"></param>
		protected override void AddSoapParams(Action<string, object> addParam)
		{
			addParam("ReservationId", m_ReservationId);
		}
	}
}
