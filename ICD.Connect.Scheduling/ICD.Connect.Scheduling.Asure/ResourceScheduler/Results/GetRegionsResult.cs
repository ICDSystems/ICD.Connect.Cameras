using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Model;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public sealed class GetRegionsResult : AbstractResult
	{
		[PublicAPI]
		public RegionSummaryData[] RegionSummaryData { get; private set; }

		/// <summary>
		/// Instantiates the GetRegionsResult from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static GetRegionsResult FromXml(string xml)
		{
			GetRegionsResult output = new GetRegionsResult
			{
				RegionSummaryData = XmlUtils.GetChildElementsAsString(xml, Model.RegionSummaryData.ELEMENT)
				                            .Select(x => Model.RegionSummaryData.FromXml(x))
				                            .ToArray()
			};

			ParseXml(output, xml);
			return output;
		}
	}
}
