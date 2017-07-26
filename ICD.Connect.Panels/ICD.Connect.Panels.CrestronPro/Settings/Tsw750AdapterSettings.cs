using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.SimplSharp.Common.UiPro.TriListAdapters;

namespace ICD.SimplSharp.Common.UiPro.Settings
{
	/// <summary>
	/// Settings for the Tsw750Adapter panel device.
	/// </summary>
	public sealed class Tsw750AdapterSettings : AbstractTriListAdapterSettings
	{
		private const string FACTORY_NAME = "Tsw750";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(Tsw750Adapter); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlPanelSettingsFactoryMethod(FACTORY_NAME)]
		public static Tsw750AdapterSettings FromXml(string xml)
		{
			Tsw750AdapterSettings output = new Tsw750AdapterSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}
