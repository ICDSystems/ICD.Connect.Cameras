using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Model;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class GetLocationsResult : AbstractResult
	{
		[PublicAPI]
		public LocationSummaryData[] LocationSummaryData { get; private set; }

		/// <summary>
		/// Instantiates the GetLocationsResult from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static GetLocationsResult FromXml(string xml)
		{
			GetLocationsResult output = new GetLocationsResult
			{
				LocationSummaryData = XmlUtils.GetChildElementsAsString(xml, Model.LocationSummaryData.ELEMENT)
				                              .Select(x => Model.LocationSummaryData.FromXml(x))
				                              .ToArray()
			};

			ParseXml(output, xml);
			return output;
		}
	}
}
