using ICD.Common.Properties;

namespace ICD.Connect.Cameras
{
	/// <summary>
	/// CameraPreset provides information about a camera preset.
	/// </summary>
	public struct CameraPreset
	{
		private readonly int m_PresetId;
		private readonly string m_Name;

		#region Region Properties

		/// <summary>
		/// Gets the preset id.
		/// </summary>
		public int PresetId { get { return m_PresetId; } }

		/// <summary>
		/// Gets the name.
		/// </summary>
		[PublicAPI]
		public string Name { get { return m_Name; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="presetId"></param>
		/// <param name="name"></param>
		public CameraPreset(int presetId, string name)
		{
			m_PresetId = presetId;
			m_Name = name;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Implementing default equality.
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		public static bool operator ==(CameraPreset s1, CameraPreset s2)
		{
			return s1.Equals(s2);
		}

		/// <summary>
		/// Implementing default inequality.
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		public static bool operator !=(CameraPreset s1, CameraPreset s2)
		{
			return !(s1 == s2);
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

			return GetHashCode() == ((CameraPreset)other).GetHashCode();
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
				hash = hash * 23 + m_PresetId;
				hash = hash * 23 + (m_Name == null ? 0 : m_Name.GetHashCode());
				return hash;
			}
		}

		#endregion
	}
}
