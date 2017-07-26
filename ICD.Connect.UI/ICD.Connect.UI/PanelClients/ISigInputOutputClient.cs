using System;
using ICD.Common.Properties;
using ICD.Connect.Panels;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.UI.PanelClients
{
	public interface ISigInputOutputClient
	{
		/// <summary>
		/// Registers the callback for input sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		[PublicAPI]
		void RegisterInputSigChangeCallback(uint number, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback);

		/// <summary>
		/// Unregisters the callback for input sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		[PublicAPI]
		void UnregisterInputSigChangeCallback(uint number, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback);

		/// <summary>
		/// Sends the serial data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="text"></param>
		[PublicAPI]
		void SendOutputSerial(uint number, string text);

		/// <summary>
		/// Sends the analog data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		[PublicAPI]
		void SendOutputAnalog(uint number, ushort value);

		/// <summary>
		/// Sends the digital data to the host.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		[PublicAPI]
		void SendOutputDigital(uint number, bool value);
	}
}
