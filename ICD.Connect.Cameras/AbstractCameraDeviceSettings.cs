﻿using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Cameras
{
	public abstract class AbstractCameraDeviceSettings : AbstractDeviceSettings, ICameraDeviceSettings
	{
		private const string PORT_ELEMENT = "Port";

		[OriginatorIdSettingsProperty(typeof(ISerialPort))]
		public int? Port { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);
			
			Port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);
		}
	}
}
