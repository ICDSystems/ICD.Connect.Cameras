using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Attributes.Factories;

namespace ICD.MetLife.RoomOS.Endpoints.Sources
{
	public sealed class MetlifeSourceSettings : AbstractSourceSettings
	{
		private const string FACTORY_NAME = "MetlifeSource";

		private const string SOURCE_TYPE_ELEMENT = "SourceType";
		private const string SOURCE_FLAGS_ELEMENT = "SourceFlags";
		private const string ENABLE_WHEN_NOT_TRANSMITTING_ELEMENT = "EnableWhenNotTransmitting";
		private const string INHIBIT_AUTO_ROUTE_ELEMENT = "InhibitAutoRoute";
		private const string INHIBIT_AUTO_UNROUTE_ELEMENT = "InhibitAutoUnroute";

		#region Properties

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(MetlifeSource); } }

		/// <summary>
		/// Type of source, mainly for icon
		/// </summary>
		public eSourceType SourceType { get; set; }

		/// <summary>
		/// Flags to determine where this source is displayed
		/// </summary>
		public eSourceFlags SourceFlags { get; set; }

		/// <summary>
		/// Determines the detection state of the source in the UI.
		/// Typically a device is only available to share when it is actively transmitting on the given output.
		/// Some devices (Clickshare) use this property to enable sharing even if the device is only transmitting
		/// a blank signal (no buttons transmitting). 
		/// </summary>
		public bool EnableWhenNotTransmitting { get; set; }

		/// <summary>
		/// When true the source is not considered for automatic routing upon detection by the system.
		/// </summary>
		public bool InhibitAutoRoute { get; set; }

		/// <summary>
		/// When true the system will not automatically unroute the source when a signal is no longer detected.
		/// </summary>
		public bool InhibitAutoUnroute { get; set; }

		#endregion

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(SOURCE_TYPE_ELEMENT, SourceType.ToString());
			writer.WriteElementString(SOURCE_FLAGS_ELEMENT, SourceFlags.ToString());
			writer.WriteElementString(ENABLE_WHEN_NOT_TRANSMITTING_ELEMENT, IcdXmlConvert.ToString(EnableWhenNotTransmitting));
			writer.WriteElementString(INHIBIT_AUTO_ROUTE_ELEMENT, IcdXmlConvert.ToString(InhibitAutoRoute));
			writer.WriteElementString(INHIBIT_AUTO_UNROUTE_ELEMENT, IcdXmlConvert.ToString(InhibitAutoUnroute));
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlSourceSettingsFactoryMethod(FACTORY_NAME)]
		public static MetlifeSourceSettings FromXml(string xml)
		{
			MetlifeSourceSettings output = new MetlifeSourceSettings();

			// Handle the legacy format, where sources were simply the device id
			int device;
			if (!TryUtils.Try(XmlUtils.ReadChildElementContentAsInt, xml, DEVICE_ELEMENT, out device))
			{
				if (!TryUtils.Try(int.Parse, XmlUtils.ReadElementContent(xml), out device))
					throw new FormatException("Source XML is not in a recognized format");

				output.Device = device;
				output.Control = 0;
				output.Address = 1;
				output.SourceType = eSourceType.Laptop;
				output.SourceFlags = eSourceFlags.Share;

				return output;
			}

			string typeString = XmlUtils.TryReadChildElementContentAsString(xml, SOURCE_TYPE_ELEMENT);
			string flagsString = XmlUtils.TryReadChildElementContentAsString(xml, SOURCE_FLAGS_ELEMENT);
			bool enableWhenNotTransmitting =
				XmlUtils.TryReadChildElementContentAsBoolean(xml, ENABLE_WHEN_NOT_TRANSMITTING_ELEMENT) ?? false;
			bool inhibitAutoRoute =
				XmlUtils.TryReadChildElementContentAsBoolean(xml, INHIBIT_AUTO_ROUTE_ELEMENT) ?? false;
			bool inhibitAutoUnroute =
				XmlUtils.TryReadChildElementContentAsBoolean(xml, INHIBIT_AUTO_UNROUTE_ELEMENT) ?? false;

			eSourceType sourceType;
			EnumUtils.TryParse(typeString, true, out sourceType);

			eSourceFlags sourceFlags;
			if (!EnumUtils.TryParse(flagsString, true, out sourceFlags))
				sourceFlags = eSourceFlags.Share;

			output.SourceType = sourceType;
			output.SourceFlags = sourceFlags;
			output.EnableWhenNotTransmitting = enableWhenNotTransmitting;
			output.InhibitAutoRoute = inhibitAutoRoute;
			output.InhibitAutoUnroute = inhibitAutoUnroute;

			ParseXml(output, xml);
			return output;
		}
	}
}
