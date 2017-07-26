namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class GetReservationsResult : AbstractReservationsResult
	{
		/// <summary>
		/// Instantiates the ReservationResult from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static GetReservationsResult FromXml(string xml)
		{
			GetReservationsResult output = new GetReservationsResult();
			ParseXml(output, xml);
			return output;
		}
	}
}
