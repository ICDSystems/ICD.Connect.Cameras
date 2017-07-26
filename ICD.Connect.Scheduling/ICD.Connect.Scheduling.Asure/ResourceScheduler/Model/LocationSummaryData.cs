using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Model
{
	public sealed class LocationSummaryData : AbstractData
	{
		public const string ELEMENT = "LocationSummaryData";

		[PublicAPI]
		public int Id { get; private set; }

		[PublicAPI]
		public string Description { get; private set; }

		[PublicAPI]
		public bool IsDefaultLocation { get; private set; }

		/// <summary>
		/// Instantiates the LocationSummaryData from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static LocationSummaryData FromXml(string xml)
		{
			return new LocationSummaryData
			{
				Id = XmlUtils.ReadChildElementContentAsInt(xml, "Id"),
				Description = XmlUtils.ReadChildElementContentAsString(xml, "Description"),
				IsDefaultLocation = XmlUtils.ReadChildElementContentAsBoolean(xml, "IsDefaultLocation")
			};
		}
	}
}
