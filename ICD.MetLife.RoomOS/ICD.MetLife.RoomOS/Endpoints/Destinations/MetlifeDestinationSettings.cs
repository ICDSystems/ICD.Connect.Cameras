using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Settings.Attributes.Factories;

namespace ICD.MetLife.RoomOS.Endpoints.Destinations
{
	public sealed class MetlifeDestinationSettings : AbstractDestinationSettings
	{
		private const string FACTORY_NAME = "MetlifeDestination";

		private const string VTC_OPTION_ELEMENT = "VtcOption";
		private const string AUDIO_OPTION_ELEMENT = "AudioOption";
		private const string SHARE_BY_DEFAULT_ELEMENT = "ShareByDefault";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(MetlifeDestination); } }

		/// <summary>
		/// Describes which VTC source is routed to this destination.
		/// </summary>
		public MetlifeDestination.eVtcOption VtcOption { get; set; }

		/// <summary>
		/// Describes which audio is routed to this destination.
		/// </summary>
		public MetlifeDestination.eAudioOption AudioOption { get; set; }

		/// <summary>
		/// Determines if a source is automatically routed to this destination when the share button is
		/// pressed, or the source is detected by the system.
		/// </summary>
		public bool ShareByDefault { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(VTC_OPTION_ELEMENT, VtcOption.ToString());
			writer.WriteElementString(AUDIO_OPTION_ELEMENT, AudioOption.ToString());
			writer.WriteElementString(SHARE_BY_DEFAULT_ELEMENT, IcdXmlConvert.ToString(ShareByDefault));
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlDestinationSettingsFactoryMethod(FACTORY_NAME)]
		public static MetlifeDestinationSettings FromXml(string xml)
		{
			string vtcOptionString = XmlUtils.TryReadChildElementContentAsString(xml, VTC_OPTION_ELEMENT);
			MetlifeDestination.eVtcOption vtcOption =
				vtcOptionString == null
					? MetlifeDestination.eVtcOption.Main
					: EnumUtils.Parse<MetlifeDestination.eVtcOption>(vtcOptionString, true);

			string audioOptionString = XmlUtils.TryReadChildElementContentAsString(xml, AUDIO_OPTION_ELEMENT);
			MetlifeDestination.eAudioOption audioOption =
				audioOptionString == null
					? MetlifeDestination.eAudioOption.Program | MetlifeDestination.eAudioOption.Call
					: EnumUtils.Parse<MetlifeDestination.eAudioOption>(audioOptionString, true);

			bool shareByDefault = XmlUtils.TryReadChildElementContentAsBoolean(xml, SHARE_BY_DEFAULT_ELEMENT) ?? true;

			MetlifeDestinationSettings output = new MetlifeDestinationSettings
			{
				VtcOption = vtcOption,
				AudioOption = audioOption,
				ShareByDefault = shareByDefault
			};

			ParseXml(output, xml);
			return output;
		}
	}
}
