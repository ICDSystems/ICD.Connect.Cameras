using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Requests
{
	public sealed class DeleteReservationRequest : AbstractRequest<DeleteReservationResult>
	{
		private const string ACTION = "DeleteReservation";

		private const string BODY_TEMPLATE = @"<x:Body>
        <ns:DeleteReservation>
            <ns:data>
                <ns:ReservationBaseData>
                    <ns:Id>{0}</ns:Id>
                </ns:ReservationBaseData>
            </ns:data>
        </ns:DeleteReservation>
    </x:Body>";

		private readonly int m_ReservationId;

		/// <summary>
		/// Gets the name of the service method.
		/// </summary>
		protected override string SoapAction { get { return ACTION; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reservationId"></param>
		public DeleteReservationRequest(int reservationId)
		{
			m_ReservationId = reservationId;
		}

		/// <summary>
		/// Gets the body xml for the request.
		/// </summary>
		/// <returns></returns>
		protected override string GetBody()
		{
			return string.Format(BODY_TEMPLATE, m_ReservationId);
		}

		/// <summary>
		/// Builds the resulting object from the xml response.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override DeleteReservationResult ResultFromXml(string xml)
		{
			return DeleteReservationResult.FromXml(xml);
		}
	}
}
