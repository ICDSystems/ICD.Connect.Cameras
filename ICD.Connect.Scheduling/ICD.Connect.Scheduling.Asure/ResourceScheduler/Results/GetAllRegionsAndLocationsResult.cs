using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Model;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class GetAllRegionsAndLocationsResult : AbstractResult
	{
		[PublicAPI]
		public RegionWithLocationsSummaryData[] RegionWithLocationsSummaryData { get; private set; }

		/// <summary>
		/// Instantiates a GetAllRegionsAndLocationsResult from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static GetAllRegionsAndLocationsResult FromXml(string xml)
		{
			GetAllRegionsAndLocationsResult output = new GetAllRegionsAndLocationsResult
			{
				RegionWithLocationsSummaryData =
					XmlUtils.GetChildElementsAsString(xml, Model.RegionWithLocationsSummaryData.ELEMENT)
					        .Select(x => Model.RegionWithLocationsSummaryData.FromXml(x))
					        .ToArray()
			};

			ParseXml(output, xml);
			return output;
		}
	}
}
