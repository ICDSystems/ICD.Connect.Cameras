using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Cameras.Panasonic
{
	[KrangSettings("PanasonicCamera", typeof(PanasonicCameraAwDevice))]
	public sealed class PanasonicCameraAwDeviceSettings : AbstractCameraDeviceSettings, IUriProperties
	{
		private const string PORT_ELEMENT = "Port";
		private const string PAN_TILT_SPEED_ELEMENT = "PanTiltSpeed";
		private const string ZOOM_SPEED_ELEMENT = "ZoomSpeed";

		private readonly UriProperties m_UriProperties;

		private int? m_PanTiltSpeed;
		private int? m_ZoomSpeed;

		#region Properties

		[OriginatorIdSettingsProperty(typeof(IWebPort))]
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

		#endregion

		#region URI

		/// <summary>
		/// Gets/sets the configurable username.
		/// </summary>
		public string Username { get { return m_UriProperties.Username; } set { m_UriProperties.Username = value; } }

		/// <summary>
		/// Gets/sets the configurable password.
		/// </summary>
		public string Password { get { return m_UriProperties.Password; } set { m_UriProperties.Password = value; } }

		/// <summary>
		/// Gets/sets the configurable network address.
		/// </summary>
		public string NetworkAddress { get { return m_UriProperties.NetworkAddress; } set { m_UriProperties.NetworkAddress = value; } }

		/// <summary>
		/// Gets/sets the configurable network port.
		/// </summary>
		public ushort NetworkPort { get { return m_UriProperties.NetworkPort; } set { m_UriProperties.NetworkPort = value; } }

		/// <summary>
		/// Gets/sets the configurable URI scheme.
		/// </summary>
		public string UriScheme { get { return m_UriProperties.UriScheme; } set { m_UriProperties.UriScheme = value; } }

		/// <summary>
		/// Gets/sets the configurable URI path.
		/// </summary>
		public string UriPath { get { return m_UriProperties.UriPath; } set { m_UriProperties.UriPath = value; } }

		/// <summary>
		/// Gets/sets the configurable URI query.
		/// </summary>
		public string UriQuery { get { return m_UriProperties.UriQuery; } set { m_UriProperties.UriQuery = value; } }

		/// <summary>
		/// Gets/sets the configurable URI fragment.
		/// </summary>
		public string UriFragment { get { return m_UriProperties.UriFragment; } set { m_UriProperties.UriFragment = value; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public PanasonicCameraAwDeviceSettings()
		{
			m_UriProperties = new UriProperties();
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));
			writer.WriteElementString(PAN_TILT_SPEED_ELEMENT, IcdXmlConvert.ToString(PanTiltSpeed));
			writer.WriteElementString(ZOOM_SPEED_ELEMENT, IcdXmlConvert.ToString(ZoomSpeed));

			m_UriProperties.WriteElements(writer);
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

			m_UriProperties.ParseXml(xml);
		}
	}
}
