using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Cameras.Mock
{
	[KrangSettings("MockCamera", typeof(MockCameraDevice))]
	public sealed class MockCameraDeviceSettings : AbstractCameraDeviceSettings
	{
		private const string PAN_TILT_SPEED_ELEMENT = "PanTiltSpeed";
		private const string ZOOM_SPEED_ELEMENT = "ZoomSpeed";

		private int? m_PanTiltSpeed;
		private int? m_ZoomSpeed;

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
					m_PanTiltSpeed = MathUtils.Clamp(value.Value, 0, 10);
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
					m_ZoomSpeed = MathUtils.Clamp(value.Value, 0, 10);
				}
			}
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			PanTiltSpeed = XmlUtils.TryReadChildElementContentAsInt(xml, PAN_TILT_SPEED_ELEMENT);
			ZoomSpeed = XmlUtils.TryReadChildElementContentAsInt(xml, ZOOM_SPEED_ELEMENT);
		}
	}
}
