using System;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.SmartObjects
{
	public abstract class AbstractSmartObject : ISmartObject
	{
		public abstract event EventHandler OnAnyOutput;

		#region Properties

		public abstract uint SmartObjectId { get; }

		/// <summary>
		/// Gets the time that the user last interacted with the panel.
		/// </summary>
		public abstract DateTime? LastOutput { get; }

		protected ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		#endregion

		#region Methods

		/// <summary>
		/// Registers the callback for output sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		public abstract void RegisterOutputSigChangeCallback(uint number, eSigType type,
		                                                     Action<SigCallbackManager, SigAdapterEventArgs> callback);

		/// <summary>
		/// Unregisters the callback for output sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		public abstract void UnregisterOutputSigChangeCallback(uint number, eSigType type,
		                                                       Action<SigCallbackManager, SigAdapterEventArgs> callback);

		/// <summary>
		/// Sends the serial data to the panel.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="text"></param>
		public abstract void SendInputSerial(uint number, string text);

		/// <summary>
		/// Sends the analog data to the panel.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public abstract void SendInputAnalog(uint number, ushort value);

		/// <summary>
		/// Sends the digital data to the panel.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public abstract void SendInputDigital(uint number, bool value);

		#endregion
	}
}
