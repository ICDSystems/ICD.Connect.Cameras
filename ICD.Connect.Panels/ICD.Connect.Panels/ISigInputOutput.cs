using System;
using ICD.Common.Properties;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels
{
	public interface ISigInputOutput
	{
		/// <summary>
		/// Raised when the user interacts with the panel.
		/// </summary>
		[PublicAPI]
		event EventHandler OnAnyOutput;

		#region Properties

		/// <summary>
		/// Gets the time that the user last interacted with the panel.
		/// </summary>
		[PublicAPI]
		DateTime? LastOutput { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Registers the callback for output sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		[PublicAPI]
		void RegisterOutputSigChangeCallback(uint number, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback);

		/// <summary>
		/// Unregisters the callback for output sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		[PublicAPI]
		void UnregisterOutputSigChangeCallback(uint number, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback);

		/// <summary>
		/// Sends the serial data to the panel.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="text"></param>
		[PublicAPI]
		void SendInputSerial(uint number, string text);

		/// <summary>
		/// Sends the analog data to the panel.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		[PublicAPI]
		void SendInputAnalog(uint number, ushort value);

		/// <summary>
		/// Sends the digital data to the panel.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		[PublicAPI]
		void SendInputDigital(uint number, bool value);

		#endregion
	}
}
