using System;
using ICD.Common.Properties;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Panels.Mock
{
	public sealed class MockPanelDeviceSettings : AbstractPanelDeviceSettings
	{
		private const string FACTORY_NAME = "MockPanel";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		public override Type OriginatorType
		{
			get { return typeof(MockPanelDevice); }
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlPanelSettingsFactoryMethod(FACTORY_NAME)]
		public static MockPanelDeviceSettings FromXml(string xml)
		{
			MockPanelDeviceSettings output = new MockPanelDeviceSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}
