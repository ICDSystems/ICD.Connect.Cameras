using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.UI.PanelClients.ThinClient
{
	public sealed class ThinSmartObjectClient : AbstractSmartObjectClient
	{
		private readonly ThinPanelClient m_Parent;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="smartObjectId"></param>
		public ThinSmartObjectClient(ThinPanelClient parent, uint smartObjectId)
			: base(smartObjectId)
		{
			m_Parent = parent;
		}

		/// <summary>
		/// Raises the events for this input sig.
		/// </summary>
		/// <param name="sig"></param>
		public void HandleInputSig(ISig sig)
		{
			RaiseInputSigChangeCallback(sig);
		}

		/// <summary>
		/// Sends the serial data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="text"></param>
		public override void SendOutputSerial(uint number, string text)
		{
			m_Parent.SendOutputSerial(SmartObjectId, number, text);
		}

		/// <summary>
		/// Sends the analog data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public override void SendOutputAnalog(uint number, ushort value)
		{
			m_Parent.SendOutputAnalog(SmartObjectId, number, value);
		}

		/// <summary>
		/// Sends the digital data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public override void SendOutputDigital(uint number, bool value)
		{
			m_Parent.SendOutputDigital(SmartObjectId, number, value);
		}
	}
}
