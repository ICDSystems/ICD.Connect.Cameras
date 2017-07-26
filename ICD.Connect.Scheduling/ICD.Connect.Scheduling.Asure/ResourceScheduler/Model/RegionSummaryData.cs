using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Model
{
	public sealed class RegionSummaryData : AbstractData
	{
		public const string ELEMENT = "RegionSummaryData";

		[PublicAPI]
		public int Id { get; private set; }

		[PublicAPI]
		public string Description { get; private set; }

		/// <summary>
		/// Instantiates the RegionSummaryData from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static RegionSummaryData FromXml(string xml)
		{
			return new RegionSummaryData
			{
				Id = XmlUtils.ReadChildElementContentAsInt(xml, "Id"),
				Description = XmlUtils.ReadChildElementContentAsString(xml, "Description")
			};
		}
	}
}
