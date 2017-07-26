using System;
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Panels.SigCollections;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.Mock
{
	public sealed class MockSmartObject : AbstractSmartObject
	{
		public override event EventHandler OnAnyOutput;

		private readonly SigCallbackManager m_SigCallbacks;

		private IDeviceBooleanInputCollection m_BooleanInput;
		private IDeviceUShortInputCollection m_UShortInput;
		private IDeviceStringInputCollection m_StringInput;
		private uint m_SmartObjectId;

		#region Properties

		/// <summary>
		/// Collection of Boolean Inputs sent to the device.
		/// 
		/// </summary>
		public IDeviceBooleanInputCollection BooleanInput { get { return m_BooleanInput; } }

		/// <summary>
		/// Collection of Integer Inputs sent to the device.
		/// 
		/// </summary>
		public IDeviceUShortInputCollection UShortInput { get { return m_UShortInput; } }

		/// <summary>
		/// Collection of String Inputs sent to the device.
		/// 
		/// </summary>
		public IDeviceStringInputCollection StringInput { get { return m_StringInput; } }

		/// <summary>
		/// 
		/// </summary>
		public override uint SmartObjectId { get { return m_SmartObjectId; } }

		/// <summary>
		/// Gets the time that the user last interacted with the panel.
		/// </summary>
		public override DateTime? LastOutput { get { throw new NotImplementedException(); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public MockSmartObject()
		{
			m_SigCallbacks = new SigCallbackManager();
		}

		#region Methods

		/// <summary>
		/// Registers the callback for output sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		public override void RegisterOutputSigChangeCallback(uint number, eSigType type,
		                                                     Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			m_SigCallbacks.RegisterSigChangeCallback(number, type, callback);
		}

		/// <summary>
		/// Unregisters the callback for output sig change events.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="type"></param>
		/// <param name="callback"></param>
		public override void UnregisterOutputSigChangeCallback(uint number, eSigType type,
		                                                       Action<SigCallbackManager, SigAdapterEventArgs> callback)
		{
			m_SigCallbacks.UnregisterSigChangeCallback(number, type, callback);
		}

		/// <summary>
		/// Sends the serial data to the panel.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="text"></param>
		public override void SendInputSerial(uint number, string text)
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
		public override void SendInputAnalog(uint number, ushort value)
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
		public override void SendInputDigital(uint number, bool value)
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

		/// <summary>
		/// Raises the sig change callbacks.
		/// </summary>
		/// <param name="sig"></param>
		[PublicAPI]
		public void RaiseOutputSigChange(ISig sig)
		{
			m_SigCallbacks.RaiseSigChangeCallback(sig);
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
		public void SetSmartObjectId(uint id)
		{
			m_SmartObjectId = id;
		}

		#endregion

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
	}
}
