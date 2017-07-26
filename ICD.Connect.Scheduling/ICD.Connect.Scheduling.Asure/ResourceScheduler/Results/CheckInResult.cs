namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class CheckInResult : AbstractReservationResult
	{
		/// <summary>
		/// Instantiates the CheckInResult from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static CheckInResult FromXml(string xml)
		{
			CheckInResult output = new CheckInResult();
			ParseXml(output, xml);

			return output;
		}
	}
}
