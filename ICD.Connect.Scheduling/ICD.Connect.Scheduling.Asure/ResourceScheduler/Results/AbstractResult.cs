using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Model;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Results
{
	public abstract class AbstractResult
	{
		[PublicAPI]
		public bool IsValid { get; private set; }

		[PublicAPI]
		public BrokenRuleData[] BrokenBusinessRules { get; private set; }

		[PublicAPI]
		public BrokenRuleData[] AllChildBrokenBusinessRules { get; private set; }

		/// <summary>
		/// Parses the xml for common properties.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected static void ParseXml(AbstractResult instance, string xml)
		{
			instance.IsValid = XmlUtils.TryReadChildElementContentAsBoolean(xml, "IsValid") ?? true;

			instance.BrokenBusinessRules = GetBrokenRuleDataFromXml(xml, "BrokenBusinessRules");
			instance.AllChildBrokenBusinessRules = GetBrokenRuleDataFromXml(xml, "AllChildBrokenBusinessRules");
		}

		/// <summary>
		/// Parses BrokenRuleData instances from the element with the given name.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		private static BrokenRuleData[] GetBrokenRuleDataFromXml(string xml, string element)
		{
			try
			{
				string elementXml = XmlUtils.GetChildElementAsString(xml, element);
				return XmlUtils.GetChildElementsAsString(elementXml, BrokenRuleData.ELEMENT)
				               .Select(x => BrokenRuleData.FromXml(x))
				               .ToArray();
			}
			catch (Exception)
			{
				return new BrokenRuleData[0];
			}
		}
	}
}
