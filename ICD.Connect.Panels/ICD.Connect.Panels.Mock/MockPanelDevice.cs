using ICD.Common.Properties;
using ICD.Connect.Panels.SigCollections;
using ICD.Connect.Panels.SmartObjectCollections;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.Mock
{
	/// <summary>
	/// MockPanelDevice provides a way for us to test UI code without using a panel.
	/// </summary>
	public sealed class MockPanelDevice : AbstractPanelDevice<MockPanelDeviceSettings>
	{
		private IDeviceBooleanInputCollection m_BooleanInput;
		private IDeviceUShortInputCollection m_UShortInput;
		private IDeviceStringInputCollection m_StringInput;
		private ISmartObjectCollection m_SmartObjects;

		#region Properties

		/// <summary>
		/// Collection of Boolean Inputs sent to the device.
		/// </summary>
		protected override IDeviceBooleanInputCollection BooleanInput { get { return m_BooleanInput; } }

		/// <summary>
		/// Collection of Integer Inputs sent to the device.
		/// </summary>
		protected override IDeviceUShortInputCollection UShortInput { get { return m_UShortInput; } }

		/// <summary>
		/// Collection of String Inputs sent to the device.
		/// </summary>
		protected override IDeviceStringInputCollection StringInput { get { return m_StringInput; } }

		/// <summary>
		/// Collection containing the loaded SmartObjects of this device.
		/// </summary>
		public override ISmartObjectCollection SmartObjects { get { return m_SmartObjects; } }

		#endregion

		#region Methods

		/// <summary>
		/// Raises the sig change callbacks.
		/// </summary>
		/// <param name="sig"></param>
		[PublicAPI]
		public void RaiseOutputSigChange(ISig sig)
		{
			RaiseOutputSigChangeCallback(sig);
		}

		[PublicAPI]
		public void SetBooleanInput(IDeviceBooleanInputCollection collection)
		{
			m_BooleanInput = collection;
		}

		[PublicAPI]
		public void SetUShortInput(IDeviceUShortInputCollection collection)
		{
			m_UShortInput = collection;
		}

		[PublicAPI]
		public void SetStringInput(IDeviceStringInputCollection collection)
		{
			m_StringInput = collection;
		}

		[PublicAPI]
		public void SetSmartObjects(ISmartObjectCollection collection)
		{
			m_SmartObjects = collection;
		}

		#endregion

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return true;
		}
	}
}
