namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class GetReservationsByAttendeeResult : AbstractReservationsResult
	{
		/// <summary>
		/// Instantiates a GetReservationsByAttendeeResult from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static GetReservationsByAttendeeResult FromXml(string xml)
		{
			GetReservationsByAttendeeResult output = new GetReservationsByAttendeeResult();
			ParseXml(output, xml);
			return output;
		}
	}
}
