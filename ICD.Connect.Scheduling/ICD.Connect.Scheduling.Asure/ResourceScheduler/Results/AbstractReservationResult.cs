using ICD.Common.Properties;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Model;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public abstract class AbstractReservationResult : AbstractResult
	{
		[PublicAPI]
		public ReservationData ReservationData { get; private set; }

		protected static void ParseXml(AbstractReservationResult instance, string xml)
		{
			instance.ReservationData = ReservationData.FromXml(xml);

			AbstractResult.ParseXml(instance, xml);
		}
	}
}
