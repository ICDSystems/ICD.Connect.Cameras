namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class GetReservationsByLocationResult : AbstractReservationsResult
	{
		/// <summary>
		/// Gets a GetReservationsByLocationResult instance from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static GetReservationsByLocationResult FromXml(string xml)
		{
			GetReservationsByLocationResult output = new GetReservationsByLocationResult();
			ParseXml(output, xml);
			return output;
		}
	}
}
