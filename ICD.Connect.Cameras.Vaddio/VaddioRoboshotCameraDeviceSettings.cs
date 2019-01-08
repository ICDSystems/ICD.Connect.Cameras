using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Cameras.Vaddio
{
	[KrangSettings("VaddioRoboshot", typeof(VaddioRoboshotCameraDevice))]
	public sealed class VaddioRoboshotCameraDeviceSettings : AbstractCameraDeviceSettings, INetworkSettings
	{
		private const string PORT_ELEMENT = "Port";
		private const string USERNAME_ELEMENT = "Username";
		private const string PASSWORD_ELEMENT = "Password";
		private const string PAN_SPEED_ELEMENT = "PanSpeed";
		private const string TILT_SPEED_ELEMENT = "TiltSpeed";
		private const string ZOOM_SPEED_ELEMENT = "ZoomSpeed";

		private readonly NetworkProperties m_NetworkProperties;

		private int? m_PanSpeed;
		private int? m_TiltSpeed;
		private int? m_ZoomSpeed;

		#region Properties

		[OriginatorIdSettingsProperty(typeof(ISerialPort))]
		public int? Port { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public int? PanSpeed
		{
			get { return m_PanSpeed; }
			set
			{
				if (value == null)
				{
					m_PanSpeed = null;
				}
				else
				{
					m_PanSpeed = MathUtils.Clamp(value.Value, 1, 24);
				}
			}
		}

		public int? TiltSpeed
		{
			get { return m_TiltSpeed; }
			set
			{
				if (value == null)
				{
					m_TiltSpeed = null;
				}
				else
				{
					m_TiltSpeed = MathUtils.Clamp(value.Value, 1, 20);
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
					m_ZoomSpeed = MathUtils.Clamp(value.Value, 1, 7);
				}
			}
		}

		#endregion

		#region Network

		/// <summary>
		/// Gets/sets the configurable network address.
		/// </summary>
		public string NetworkAddress
		{
			get { return m_NetworkProperties.NetworkAddress; }
			set { m_NetworkProperties.NetworkAddress = value; }
		}

		/// <summary>
		/// Gets/sets the configurable network port.
		/// </summary>
		public ushort? NetworkPort
		{
			get { return m_NetworkProperties.NetworkPort; }
			set { m_NetworkProperties.NetworkPort = value; }
		}

		/// <summary>
		/// Clears the configured values.
		/// </summary>
		void INetworkProperties.Clear()
		{
			m_NetworkProperties.Clear();
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public VaddioRoboshotCameraDeviceSettings()
		{
			m_NetworkProperties = new NetworkProperties();
		}

		/// <summary>
		/// Write settings elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));
			writer.WriteElementString(USERNAME_ELEMENT, Username);
			writer.WriteElementString(PASSWORD_ELEMENT, Password);
			writer.WriteElementString(PAN_SPEED_ELEMENT, IcdXmlConvert.ToString(PanSpeed));
			writer.WriteElementString(TILT_SPEED_ELEMENT, IcdXmlConvert.ToString(TiltSpeed));
			writer.WriteElementString(ZOOM_SPEED_ELEMENT, IcdXmlConvert.ToString(ZoomSpeed));

			m_NetworkProperties.WriteElements(writer);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);
			Username = XmlUtils.TryReadChildElementContentAsString(xml, USERNAME_ELEMENT);
			Password = XmlUtils.TryReadChildElementContentAsString(xml, PASSWORD_ELEMENT);
			PanSpeed = XmlUtils.TryReadChildElementContentAsInt(xml, PAN_SPEED_ELEMENT);
			TiltSpeed = XmlUtils.TryReadChildElementContentAsInt(xml, TILT_SPEED_ELEMENT);
			ZoomSpeed = XmlUtils.TryReadChildElementContentAsInt(xml, ZOOM_SPEED_ELEMENT);

			m_NetworkProperties.ParseXml(xml);
		}
	}
}
