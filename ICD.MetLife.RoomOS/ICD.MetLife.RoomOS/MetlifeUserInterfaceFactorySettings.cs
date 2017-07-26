using System;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Connect.UI;

namespace ICD.MetLife.RoomOS
{
	public sealed class MetlifeUserInterfaceFactorySettings : AbstractUserInterfaceFactorySettings
	{
		private const string FACTORY_NAME = "MetlifeUserInterfaceFactory";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(MetlifeUserInterfaceFactory); } }

		/// <summary>
		/// Instantiates room settings from an xml element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[XmlUserInterfaceFactorySettingsFactoryMethod(FACTORY_NAME)]
		public static MetlifeUserInterfaceFactorySettings FromXml(string xml)
		{
			MetlifeUserInterfaceFactorySettings output = new MetlifeUserInterfaceFactorySettings();
			ParseXml(output, xml);
			return output;
		}
	}
}
