namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class GetReservationResult : AbstractReservationResult
	{
		/// <summary>
		/// Instantiates the ReservationResult from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static GetReservationResult FromXml(string xml)
		{
			GetReservationResult output = new GetReservationResult();
			ParseXml(output, xml);
			return output;
		}
	}
}
