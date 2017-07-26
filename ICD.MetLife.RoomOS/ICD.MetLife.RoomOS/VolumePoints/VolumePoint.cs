using ICD.Common.Utils.Xml;

namespace ICD.MetLife.RoomOS.VolumePoints
{
	/// <summary>
	/// Used by the metlife room to better manage volume controls.
	/// </summary>
	public sealed class VolumePoint
	{
		private const string VOLUME_POINT_ELEMENT = "VolumePoint";
		private const string DEVICE_ELEMENT = "Device";
		private const string CONTROL_ELEMENT = "Control";
		private const string VOLUME_TYPE_ELEMENT = "VolumeType";

		private readonly int m_DeviceId;
		private readonly int? m_ControlId;
		private readonly eVolumeType m_VolumeType;

		#region Properties

		/// <summary>
		/// Device id
		/// </summary>
		public int DeviceId { get { return m_DeviceId; } }

		/// <summary>
		/// Control id.
		/// </summary>
		public int? ControlId { get { return m_ControlId; } }

		/// <summary>
		/// Determines when this control is used contextually.
		/// </summary>
		public eVolumeType VolumeType { get { return m_VolumeType; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="controlId"></param>
		/// <param name="volumeType"></param>
		public VolumePoint(int deviceId, int? controlId, eVolumeType volumeType)
		{
			m_DeviceId = deviceId;
			m_ControlId = controlId;
			m_VolumeType = volumeType;
		}

		/// <summary>
		/// Creates a VolumePoint.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static VolumePoint FromXml(string xml)
		{
			int deviceId = XmlUtils.ReadChildElementContentAsInt(xml, DEVICE_ELEMENT);
			int? controlId = XmlUtils.TryReadChildElementContentAsInt(xml, CONTROL_ELEMENT);
			eVolumeType volumeType = XmlUtils.ReadChildElementContentAsEnum<eVolumeType>(xml, VOLUME_TYPE_ELEMENT, true);

			return new VolumePoint(deviceId, controlId, volumeType);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Output element as XML to the IcdXmlTextWriter.
		/// </summary>
		/// <param name="writer"></param>
		public void WriteElement(IcdXmlTextWriter writer)
		{
			writer.WriteStartElement(VOLUME_POINT_ELEMENT);
			{
				writer.WriteElementString(DEVICE_ELEMENT, IcdXmlConvert.ToString(m_DeviceId));

				if (ControlId != null)
					writer.WriteElementString(CONTROL_ELEMENT, IcdXmlConvert.ToString(m_ControlId));

				writer.WriteElementString(VOLUME_TYPE_ELEMENT, VolumeType.ToString());
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}
