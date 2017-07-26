using System;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Requests
{
	public sealed class GetReservationRequest : AbstractRequest<GetReservationResult>
	{
		private const string ACTION = "GetReservation";

		private readonly int m_Id;

		/// <summary>
		/// Gets the name of the service method.
		/// </summary>
		protected override string SoapAction { get { return ACTION; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="id"></param>
		public GetReservationRequest(int id)
		{
			m_Id = id;
		}

		/// <summary>
		/// Builds the parameters xml by calling addParam for each name/value pair.
		/// </summary>
		/// <param name="addParam"></param>
		protected override void AddSoapParams(Action<string, object> addParam)
		{
			addParam("ReservationId", m_Id);
		}

		/// <summary>
		/// Builds the resulting object from the xml response.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override GetReservationResult ResultFromXml(string xml)
		{
			return GetReservationResult.FromXml(xml);
		}
	}
}
