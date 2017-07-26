namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class SubmitReservationResult : AbstractReservationResult
	{
		/// <summary>
		/// Instantiates a SubmitReservationResult from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static SubmitReservationResult FromXml(string xml)
		{
			SubmitReservationResult output = new SubmitReservationResult();
			ParseXml(output, xml);
			return output;
		}
	}
}
