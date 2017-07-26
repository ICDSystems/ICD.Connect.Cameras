using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.SimplSharp.Common.UiPro.TriListAdapters;

namespace ICD.SimplSharp.Common.UiPro.Settings
{
	public sealed class Tsw1052AdapterSettings : AbstractTriListAdapterSettings
	{
		private const string FACTORY_NAME = "Tsw1052";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(Tsw1052Adapter); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlPanelSettingsFactoryMethod(FACTORY_NAME)]
		public static Tsw1052AdapterSettings FromXml(string xml)
		{
			Tsw1052AdapterSettings output = new Tsw1052AdapterSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}