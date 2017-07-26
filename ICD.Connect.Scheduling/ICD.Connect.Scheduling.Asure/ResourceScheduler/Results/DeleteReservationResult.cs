namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class DeleteReservationResult : AbstractResult
	{
		/// <summary>
		/// Instantiates a DeleteReservationResult from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static DeleteReservationResult FromXml(string xml)
		{
			DeleteReservationResult output = new DeleteReservationResult();
			ParseXml(output, xml);
			return output;
		}
	}
}
