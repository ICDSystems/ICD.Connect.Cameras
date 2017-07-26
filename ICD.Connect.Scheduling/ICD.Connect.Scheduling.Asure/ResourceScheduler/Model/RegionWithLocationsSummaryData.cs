using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Model
{
	public sealed class RegionWithLocationsSummaryData : AbstractData
	{
		public const string ELEMENT = "RegionWithLocationsSummaryData";

		[PublicAPI]
		public int Id { get; private set; }

		[PublicAPI]
		public string Description { get; private set; }

		[PublicAPI]
		public LocationSummaryData[] LocationSummaryData { get; private set; }

		/// <summary>
		/// Instantiates a RegionWithLocationsSummaryData from xml. 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static RegionWithLocationsSummaryData FromXml(string xml)
		{
			string locationsXml = XmlUtils.GetChildElementAsString(xml, "Locations");

			return new RegionWithLocationsSummaryData
			{
				Id = XmlUtils.ReadChildElementContentAsInt(xml, "Id"),
				Description = XmlUtils.ReadChildElementContentAsString(xml, "Description"),
				LocationSummaryData = XmlUtils.GetChildElementsAsString(locationsXml, Model.LocationSummaryData.ELEMENT)
				                              .Select(x => Model.LocationSummaryData.FromXml(x))
				                              .ToArray()
			};
		}
	}
}
