using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Cameras.Panasonic
{
	public sealed class PanasonicCameraAwDeviceSettings : AbstractCameraDeviceSettings
	{
		private int? m_PanTiltSpeed;
		private int? m_ZoomSpeed;
        private const string FACTORY_NAME = "PanasonicCamera";
		private const string PAN_TILT_SPEED_ELEMENT = "PanTiltSpeed";
		private const string ZOOM_SPEED_ELEMENT = "ZoomSpeed";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

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
					m_PanTiltSpeed = MathUtils.Clamp(value.Value, 0, 49);
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
					m_ZoomSpeed = MathUtils.Clamp(value.Value, 0, 49);
				}
			}
		}

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(PanasonicCameraAwDevice); } }

		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static PanasonicCameraAwDeviceSettings FromXml(string xml)
		{
			PanasonicCameraAwDeviceSettings output = new PanasonicCameraAwDeviceSettings();
			ParseXml(output, xml);
			return output;
		}

		private static void ParseXml(PanasonicCameraAwDeviceSettings instance, string xml)
		{
			instance.PanTiltSpeed = XmlUtils.TryReadChildElementContentAsInt(xml, PAN_TILT_SPEED_ELEMENT);
			instance.ZoomSpeed = XmlUtils.TryReadChildElementContentAsInt(xml, ZOOM_SPEED_ELEMENT);
			AbstractCameraDeviceSettings.ParseXml(instance, xml);
		}
	}
}
