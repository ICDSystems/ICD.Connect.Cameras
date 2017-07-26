using System;
using ICD.Connect.Panels;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.UI.PanelClients
{
	public abstract class AbstractSigInputOutputClient : ISigInputOutputClient
	{
		private readonly SigCallbackManager m_SigCallbacks;

		protected AbstractSigInputOutputClient()
		{
			m_SigCallbacks = new SigCallbackManager();
		}

		/// <summary>
		/// Raises the callbacks registered with the signature.
		/// </summary>
		/// <param name="sig"></param>
		protected void RaiseInputSigChangeCallback(ISig sig)
		{
			m_SigCallbacks.RaiseSigChangeCallback(sig);
		}

		/// <summary>
		/// Registers the callback for input sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		public void RegisterInputSigChangeCallback(uint number, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			m_SigCallbacks.RegisterSigChangeCallback(number, type, callback);
		}

		/// <summary>
		/// Unregisters the callback for input sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		public void UnregisterInputSigChangeCallback(uint number, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			m_SigCallbacks.UnregisterSigChangeCallback(number, type, callback);
		}

		/// <summary>
		/// Sends the serial data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="text"></param>
		public abstract void SendOutputSerial(uint number, string text);

		/// <summary>
		/// Sends the analog data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public abstract void SendOutputAnalog(uint number, ushort value);

		/// <summary>
		/// Sends the digital data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public abstract void SendOutputDigital(uint number, bool value);
	}
}
