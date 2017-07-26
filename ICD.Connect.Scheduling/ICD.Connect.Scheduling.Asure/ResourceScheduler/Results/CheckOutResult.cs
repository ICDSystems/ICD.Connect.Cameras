namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class CheckOutResult : AbstractReservationResult
	{
		/// <summary>
		/// Instantiates the CheckOutResult from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static CheckOutResult FromXml(string xml)
		{
			CheckOutResult output = new CheckOutResult();
			ParseXml(output, xml);

			return output;
		}
	}
}
