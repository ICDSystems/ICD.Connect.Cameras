using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Model;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public abstract class AbstractReservationsResult : AbstractResult
	{
		[PublicAPI]
		public ReservationData[] ReservationData { get; private set; }

		protected static void ParseXml(AbstractReservationsResult instance, string xml)
		{
			instance.ReservationData = XmlUtils.GetChildElementsAsString(xml, Model.ReservationData.ELEMENT)
			                                   .Select(x => Model.ReservationData.FromXml(x))
			                                   .ToArray();

			AbstractResult.ParseXml(instance, xml);
		}
	}
}
