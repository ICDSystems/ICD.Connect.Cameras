using System;
using Crestron.SimplSharpPro;
using ICD.Common.Services.Logging;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Misc.CrestronPro.Sigs;
using ICD.Connect.Panels;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Panels.SigCollections;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.Protocol.Sigs;
using eSigType = ICD.Connect.Protocol.Sigs.eSigType;

namespace ICD.SimplSharp.Common.UiPro.TriListAdapters
{
	public sealed class SmartObjectAdapter : AbstractSmartObject, IDisposable
	{
		public override event EventHandler OnAnyOutput;

		private readonly SmartObject m_SmartObject;
		private readonly SigCallbackManager m_SigCallbacks;

		private IDeviceBooleanInputCollection m_BooleanInput;
		private IDeviceUShortInputCollection m_UShortInput;
		private IDeviceStringInputCollection m_StringInput;

		#region Properties

		/// <summary>
		/// Gets the ID of the Smart Object.
		/// </summary>
		public override uint SmartObjectId { get { return m_SmartObject.ID; } }

		/// <summary>
		/// Gets the time that the user last interacted with the panel.
		/// </summary>
		public override DateTime? LastOutput { get { return m_SigCallbacks.LastOutput; } }

		/// <summary>
		/// Collection of Boolean Inputs sent to the device.
		/// </summary>
		private IDeviceBooleanInputCollection BooleanInput
		{
			get
			{
				return m_BooleanInput ?? (m_BooleanInput = new DeviceBooleanInputCollectionAdapter(m_SmartObject.BooleanInput));
			}
		}

		/// <summary>
		/// Collection of Integer Inputs sent to the device.
		/// </summary>
		private IDeviceUShortInputCollection UShortInput
		{
			get { return m_UShortInput ?? (m_UShortInput = new DeviceUShortInputCollectionAdapter(m_SmartObject.UShortInput)); }
		}

		/// <summary>
		/// Collection of String Inputs sent to the device.
		/// </summary>
		private IDeviceStringInputCollection StringInput
		{
			get { return m_StringInput ?? (m_StringInput = new DeviceStringInputCollectionAdapter(m_SmartObject.StringInput)); }
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObject"></param>
		public SmartObjectAdapter(SmartObject smartObject)
		{
			m_SigCallbacks = new SigCallbackManager();
			m_SigCallbacks.OnAnyCallback += SigCallbacksOnAnyCallback;

			m_SmartObject = smartObject;
			Subscribe(m_SmartObject);
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			Unsubscribe(m_SmartObject);
		}

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

		#endregion

		#region Private Methods

		private void SigCallbacksOnAnyCallback(object sender, EventArgs eventArgs)
		{
			OnAnyOutput.Raise(this);
		}

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

		#endregion

		#region SmartObject Callbacks

		/// <summary>
		/// Subscribe to the TriList events.
		/// </summary>
		/// <param name="smartObject"></param>
		private void Subscribe(SmartObject smartObject)
		{
			if (smartObject == null)
				return;

			smartObject.SigChange += SmartObjectOnSigChange;
		}

		/// <summary>
		/// Subscribe to the TriList events.
		/// </summary>
		/// <param name="smartObject"></param>
		private void Unsubscribe(SmartObject smartObject)
		{
			if (smartObject == null)
				return;

			smartObject.SigChange -= SmartObjectOnSigChange;
		}

		/// <summary>
		/// Called when a sig change comes from the TriList.
		/// </summary>
		/// <param name="currentDevice"></param>
		/// <param name="smartObjectEventArgs"></param>
		private void SmartObjectOnSigChange(GenericBase currentDevice, SmartObjectEventArgs smartObjectEventArgs)
		{
			m_SigCallbacks.RaiseSigChangeCallback(SigAdapterFactory.GetSigAdapter(smartObjectEventArgs.Sig));
		}

		#endregion
	}
}
