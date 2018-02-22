﻿using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Cameras.Visca
{
	[KrangSettings(FACTORY_NAME)]
	public sealed class ViscaCameraDeviceSettings : AbstractCameraDeviceSettings
	{
		private int? m_PanTiltSpeed;
		private int? m_ZoomSpeed;
		private const string FACTORY_NAME = "ViscaCamera";
		private const string PORT_ELEMENT = "Port";
		private const string PAN_TILT_SPEED_ELEMENT = "PanTiltSpeed";
		private const string ZOOM_SPEED_ELEMENT = "ZoomSpeed";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		[OriginatorIdSettingsProperty(typeof(ISerialPort))]
		public int? Port { get; set; }

		public int? PanTiltSpeed 
		{ 
			get { return m_PanTiltSpeed; }
			set
			{
				if (value == null)
				{
					m_PanTiltSpeed = null;
				}
				else
				{
					m_PanTiltSpeed = MathUtils.Clamp(value.Value, 1, 24);
				}
			} 
		}

		public int? ZoomSpeed
		{
			get { return m_ZoomSpeed; }
			set
			{
				if (value == null)
				{
					m_ZoomSpeed = null;
				}
				else
				{
					m_ZoomSpeed = MathUtils.Clamp(value.Value, 1, 20);
				}
			}
		}

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(ViscaCameraDevice); } }

		/// <summary>
		/// Write settings elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));
			writer.WriteElementString(PAN_TILT_SPEED_ELEMENT, IcdXmlConvert.ToString(PanTiltSpeed));
			writer.WriteElementString(ZOOM_SPEED_ELEMENT, IcdXmlConvert.ToString(ZoomSpeed));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);
			PanTiltSpeed = XmlUtils.TryReadChildElementContentAsInt(xml, PAN_TILT_SPEED_ELEMENT);
			ZoomSpeed = XmlUtils.TryReadChildElementContentAsInt(xml, ZOOM_SPEED_ELEMENT);
		}
	}
}
