namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class GetReservationsByResourceResult : AbstractReservationsResult
	{
		/// <summary>
		/// Instantiates a GetReservationsByResourceResult from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static GetReservationsByResourceResult FromXml(string xml)
		{
			GetReservationsByResourceResult output = new GetReservationsByResourceResult();
			ParseXml(output, xml);
			return output;
		}
	}
}
