namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class CheckInNowResult : AbstractReservationResult
	{
		/// <summary>
		/// Instantiates the CheckInNowResult from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static CheckInNowResult FromXml(string xml)
		{
			CheckInNowResult output = new CheckInNowResult();
			ParseXml(output, xml);

			return output;
		}
	}
}
