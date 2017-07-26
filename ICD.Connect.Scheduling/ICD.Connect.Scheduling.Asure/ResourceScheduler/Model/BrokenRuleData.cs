using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Model
{
	public sealed class BrokenRuleData : AbstractData
	{
		public const string ELEMENT = "BrokenRuleData";

		[PublicAPI]
		public string Property { get; private set; }

		[PublicAPI]
		public string Description { get; private set; }

		[PublicAPI]
		public string RuleName { get; private set; }

		/// <summary>
		/// Instantiates the BrokenRuleData from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static BrokenRuleData FromXml(string xml)
		{
			return new BrokenRuleData
			{
				Property = XmlUtils.ReadChildElementContentAsString(xml, "Property"),
				Description = XmlUtils.ReadChildElementContentAsString(xml, "Description"),
				RuleName = XmlUtils.ReadChildElementContentAsString(xml, "RuleName")
			};
		}
	}
}
