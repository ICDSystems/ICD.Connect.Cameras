using System;
using ICD.Common.Services.Logging;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Panels.SigCollections;
using ICD.Connect.Protocol.Sigs;
using ICD.Connect.Settings;

namespace ICD.Connect.Panels
{
	/// <summary>
	/// AbstractPanelBase represents shared functionality between the PanelDevice and the SmartObject.
	/// </summary>
	public abstract class AbstractSigDeviceBase<TSettings> : AbstractDeviceBase<TSettings>, ISigDevice
		where TSettings : ISettings, new()
	{
		/// <summary>
		/// Raised when the user interacts with the panel.
		/// </summary>
		public event EventHandler OnAnyOutput;

		private readonly SigCallbackManager m_SigCallbacks;

		#region Properties

		/// <summary>
		/// Gets the time that the user last interacted with the panel.
		/// </summary>
		public virtual DateTime? LastOutput { get { return m_SigCallbacks.LastOutput; } }

		/// <summary>
		/// Collection of Boolean Inputs sent to the device.
		/// </summary>
		protected abstract IDeviceBooleanInputCollection BooleanInput { get; }

		/// <summary>
		/// Collection of Integer Inputs sent to the device.
		/// </summary>
		protected abstract IDeviceUShortInputCollection UShortInput { get; }

		/// <summary>
		/// Collection of String Inputs sent to the device.
		/// </summary>
		protected abstract IDeviceStringInputCollection StringInput { get; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractSigDeviceBase()
		{
			m_SigCallbacks = new SigCallbackManager();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			OnAnyOutput = null;
		}

		/// <summary>
		/// Registers the callback for output sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		public void RegisterOutputSigChangeCallback(uint number, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			m_SigCallbacks.RegisterSigChangeCallback(number, type, callback);
		}

		/// <summary>
		/// Unregisters the callback for output sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		public void UnregisterOutputSigChangeCallback(uint number, eSigType type, Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			m_SigCallbacks.UnregisterSigChangeCallback(number, type, callback);
		}

		/// <summary>
		/// Sends the serial data to the panel.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="text"></param>
		public void SendInputSerial(uint number, string text)
		{
			try
			{
				SendSerial(StringInput[number], text);
			}
			catch (Exception e)
			{
				Logger.AddEntry(eSeverity.Error, e, "Unable to send serial sig {0}", number);
			}
		}

		/// <summary>
		/// Sends the analog data to the panel.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public void SendInputAnalog(uint number, ushort value)
		{
			try
			{
				SendAnalog(UShortInput[number], value);
			}
			catch (Exception e)
			{
				Logger.AddEntry(eSeverity.Error, e, "Unable to send analog sig {0}", number);
			}
		}

		/// <summary>
		/// Sends the digital data to the panel.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="value"></param>
		public void SendInputDigital(uint number, bool value)
		{
			try
			{
				SendDigital(BooleanInput[number], value);
			}
			catch (Exception e)
			{
				Logger.AddEntry(eSeverity.Error, e, "Unable to send digital sig {0}", number);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sends the serial data to the panel.
		/// </summary>
		/// <param name="sig"></param>
		/// <param name="text"></param>
		private static void SendSerial(IStringInputSig sig, string text)
		{
			sig.SetStringValue(text);
		}

		/// <summary>
		/// Sends the analog data to the panel.
		/// </summary>
		/// <param name="sig"></param>
		/// <param name="value"></param>
		private static void SendAnalog(IUShortInputSig sig, ushort value)
		{
			sig.SetUShortValue(value);
		}

		/// <summary>
		/// Sends the digital data to the panel.
		/// </summary>
		/// <param name="sig"></param>
		/// <param name="value"></param>
		private static void SendDigital(IBoolInputSig sig, bool value)
		{
			sig.SetBoolValue(value);
		}

		/// <summary>
		/// Raises the callbacks registered with the signature.
		/// </summary>
		/// <param name="sig"></param>
		protected void RaiseOutputSigChangeCallback(ISig sig)
		{
			m_SigCallbacks.RaiseSigChangeCallback(sig);
		}

		/// <summary>
		/// Raises the OnAnyOutput event.
		/// </summary>
		protected void RaiseOnAnyOutput()
		{
			OnAnyOutput.Raise(this);
		}

		#endregion
	}
}
