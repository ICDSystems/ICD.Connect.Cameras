using ICD.Common.Properties;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.UI.PanelClients.ThinClient
{
	public sealed class ThinPanelClient : AbstractPanelClient
	{
		public delegate void OutputSerialCallback(ThinPanelClient sender, uint smartObjectId, uint number, string value);
		public delegate void OutputAnalogCallback(ThinPanelClient sender, uint smartObjectId, uint number, ushort value);
		public delegate void OutputDigitalCallback(ThinPanelClient sender, uint smartObjectId, uint number, bool value);

		[PublicAPI]
		public event OutputSerialCallback OnSendOutputSerial;

		[PublicAPI]
		public event OutputAnalogCallback OnSendOutputAnalog;

		[PublicAPI]
		public event OutputDigitalCallback OnSendOutputDigital;

		private readonly ThinSmartObjectClientCollection m_SmartObjectClientCollection;

		/// <summary>
		/// Collection containing the SmartObject clients.
		/// </summary>
		public override ISmartObjectClientCollection SmartObjects { get { return m_SmartObjectClientCollection; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public ThinPanelClient()
		{
			m_SmartObjectClientCollection = new ThinSmartObjectClientCollection(this);
		}

		/// <summary>
		/// Raises the events for this input sig.
		/// </summary>
		/// <param name="sig"></param>
		[PublicAPI]
		public void HandleInputSig(ISig sig)
		{
			HandleInputSig(0, sig);
		}

		/// <summary>
		/// Raises the events for this input sig.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="sig"></param>
		[PublicAPI]
		public void HandleInputSig(uint smartObjectId, ISig sig)
		{
			if (smartObjectId == 0)
				RaiseInputSigChangeCallback(sig);
			else
				((ThinSmartObjectClient)m_SmartObjectClientCollection[smartObjectId]).HandleInputSig(sig);
		}

		/// <summary>
		/// Sends the serial data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="text"></param>
		public override void SendOutputSerial(uint number, string text)
		{
			SendOutputSerial(0, number, text);
		}

		/// <summary>
		/// Sends the analog data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public override void SendOutputAnalog(uint number, ushort value)
		{
			SendOutputAnalog(0, number, value);
		}

		/// <summary>
		/// Sends the digital data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public override void SendOutputDigital(uint number, bool value)
		{
			SendOutputDigital(0, number, value);
		}

		/// <summary>
		/// Sends the serial data to the host.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="number"></param>
		/// <param name="text"></param>
		public void SendOutputSerial(uint smartObjectId, uint number, string text)
		{
			OutputSerialCallback handler = OnSendOutputSerial;
			if (handler != null)
				handler(this, smartObjectId, number, text);
		}

		/// <summary>
		/// Sends the analog data to the host.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public void SendOutputAnalog(uint smartObjectId, uint number, ushort value)
		{
			OutputAnalogCallback handler = OnSendOutputAnalog;
			if (handler != null)
				handler(this, smartObjectId, number, value);
		}

		/// <summary>
		/// Sends the digital data to the host.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public void SendOutputDigital(uint smartObjectId, uint number, bool value)
		{
			OutputDigitalCallback handler = OnSendOutputDigital;
			if (handler != null)
				handler(this, smartObjectId, number, value);
		}
	}
}
