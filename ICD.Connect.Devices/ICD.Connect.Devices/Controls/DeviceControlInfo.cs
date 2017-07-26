namespace ICD.Connect.Devices.Controls
{
	/// <summary>
	/// Simple pairing of device and control ids.
	/// </summary>
	public struct DeviceControlInfo
	{
		private readonly int m_DeviceId;
		private readonly int m_ControlId;

		/// <summary>
		/// Gets the device id.
		/// </summary>
		public int DeviceId { get { return m_DeviceId; } }

		/// <summary>
		/// Gets the control id.
		/// </summary>
		public int ControlId { get { return m_ControlId; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="deviceId"></param>
		/// <param name="controlId"></param>
		public DeviceControlInfo(int deviceId, int controlId)
		{
			m_DeviceId = deviceId;
			m_ControlId = controlId;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="control"></param>
		public DeviceControlInfo(IDeviceControl control)
			: this(control.Parent.Id, control.Id)
		{
		}

		#region Equality

		/// <summary>
		/// Implementing default equality.
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <returns></returns>
		public static bool operator ==(DeviceControlInfo a1, DeviceControlInfo a2)
		{
			return a1.Equals(a2);
		}

		/// <summary>
		/// Implementing default inequality.
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <returns></returns>
		public static bool operator !=(DeviceControlInfo a1, DeviceControlInfo a2)
		{
			return !(a1 == a2);
		}

		/// <summary>
		/// Returns true if this instance is equal to the given object.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public override bool Equals(object other)
		{
			if (other == null || GetType() != other.GetType())
				return false;

			return GetHashCode() == ((DeviceControlInfo)other).GetHashCode();
		}

		/// <summary>
		/// Gets the hashcode for this instance.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + m_DeviceId;
				hash = hash * 23 + m_ControlId;
				return hash;
			}
		}

		#endregion
	}
}
