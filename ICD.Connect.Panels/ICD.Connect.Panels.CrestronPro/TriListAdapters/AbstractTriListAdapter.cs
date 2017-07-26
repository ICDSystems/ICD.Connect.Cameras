using System;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Connect.Misc.CrestronPro;
using ICD.Connect.Misc.CrestronPro.Sigs;
using ICD.Connect.Panels;
using ICD.Connect.Panels.SigCollections;
using ICD.Connect.Panels.SmartObjectCollections;
using ICD.Connect.Settings.Core;
using ISmartObject = ICD.Connect.Panels.SmartObjects.ISmartObject;

namespace ICD.SimplSharp.Common.UiPro.TriListAdapters
{
	/// <summary>
	/// TriListAdapter wraps a TriList to provide IPanelDevice features.
	/// </summary>
	public abstract class AbstractTriListAdapter<TPanel, TSettings> : AbstractPanelDevice<TSettings>
		where TPanel : BasicTriListWithSmartObject
		where TSettings : AbstractTriListAdapterSettings, new()
	{
		private IDeviceBooleanInputCollection m_BooleanInput;
		private IDeviceUShortInputCollection m_UShortInput;
		private IDeviceStringInputCollection m_StringInput;
		private readonly SmartObjectCollectionAdapter m_SmartObjects;

		#region Properties

		/// <summary>
		/// Gets the wrapped panel instance.
		/// </summary>
		[PublicAPI]
		    public TPanel Device { get; private set; }

	    protected AbstractTriListAdapter()
	    {
	        m_SmartObjects = new SmartObjectCollectionAdapter();
	        m_SmartObjects.OnSmartObjectSubscribe += SmartObjectsOnSmartObjectSubscribe;
	        m_SmartObjects.OnSmartObjectUnsubscribe += SmartObjectsOnSmartObjectUnsubscribe;
	    }

        /// <summary>
        /// Subscribes to SmartObject Touch Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="smartObject"></param>
	    private void SmartObjectsOnSmartObjectSubscribe(object sender, ISmartObject smartObject)
	    {
	        smartObject.OnAnyOutput += SmartObjectOnAnyOutput;
	    }
        /// <summary>
        /// Unsubscribes from SmartObject Touch Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="smartObject"></param>
        private void SmartObjectsOnSmartObjectUnsubscribe(object sender, ISmartObject smartObject)
        {
            smartObject.OnAnyOutput -= SmartObjectOnAnyOutput;
        }

	    /// <summary>
		/// Collection of Boolean Inputs sent to the device.
		/// </summary>
		protected override IDeviceBooleanInputCollection BooleanInput
		{
			get { return m_BooleanInput ?? (m_BooleanInput = new DeviceBooleanInputCollectionAdapter(Device.BooleanInput)); }
		}

		/// <summary>
		/// Collection of Integer Inputs sent to the device.
		/// </summary>
		protected override IDeviceUShortInputCollection UShortInput
		{
			get { return m_UShortInput ?? (m_UShortInput = new DeviceUShortInputCollectionAdapter(Device.UShortInput)); }
		}

		/// <summary>
		/// Collection of String Inputs sent to the device.
		/// </summary>
		protected override IDeviceStringInputCollection StringInput
		{
			get { return m_StringInput ?? (m_StringInput = new DeviceStringInputCollectionAdapter(Device.StringInput)); }
		}

		/// <summary>
		/// Collection containing the loaded SmartObjects of this device.
		/// </summary>
		public override ISmartObjectCollection SmartObjects
		{
		    get
		    {   
                return m_SmartObjects;
		    }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			// Unsubscribe and unregister
			SetDevice(null);
		}

		/// <summary>
		/// Sets the wrapped panel device.
		/// </summary>
		/// <param name="device"></param>
		[PublicAPI]
		public void SetDevice(TPanel device)
		{
			Unsubscribe(Device);

			if (Device != null)
			{
				if (Device.Registered)
					Device.UnRegister();

				try
				{
					Device.Dispose();
				}
				catch
				{
				}
			}

		    m_BooleanInput = null;
			m_UShortInput = null;
			m_StringInput = null;

		    Device = device;
            m_SmartObjects.SetSmartObjects(Device == null ? null : Device.SmartObjects);

			if (Device != null && !Device.Registered)
			{
				if (Name != null)
					Device.Description = Name;

				eDeviceRegistrationUnRegistrationResponse result = Device.Register();
				if (result != eDeviceRegistrationUnRegistrationResponse.Success)
					Logger.AddEntry(eSeverity.Error, "Unable to register {0} - {1}", Device.GetType().Name, result);
			}

			Subscribe(Device);
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetDevice(null);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Ipid = (byte)Device.ID;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			TPanel triList = InstantiateTriList(settings.Ipid, ProgramInfo.ControlSystem);
			SetDevice(triList);
		}

		/// <summary>
		/// Creates an instance of the wrapped trilist.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="controlSystem"></param>
		/// <returns></returns>
		protected abstract TPanel InstantiateTriList(byte ipid, CrestronControlSystem controlSystem);

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribe to the device events.
		/// </summary>
		/// <param name="device"></param>
		private void Subscribe(TPanel device)
		{
			if (device == null)
				return;

			device.SigChange += TriListOnSigChange;
			device.OnlineStatusChange += DeviceOnLineStatusChange;
		}

		/// <summary>
		/// Subscribe to the TriList events.
		/// </summary>
		/// <param name="device"></param>
		private void Unsubscribe(TPanel device)
		{
			if (device == null)
				return;

			device.SigChange -= TriListOnSigChange;
			device.OnlineStatusChange += DeviceOnLineStatusChange;
		}

		/// <summary>
		/// Called when the device goes online/offline.
		/// </summary>
		/// <param name="currentDevice"></param>
		/// <param name="args"></param>
		private void DeviceOnLineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Called when a sig change comes from the TriList.
		/// </summary>
		/// <param name="currentDevice"></param>
		/// <param name="args"></param>
		private void TriListOnSigChange(BasicTriList currentDevice, SigEventArgs args)
		{
			RaiseOutputSigChangeCallback(SigAdapterFactory.GetSigAdapter(args.Sig));
            RaiseOnAnyOutput();
		}

		/// <summary>
		/// Called when the user interacts with a SmartObject.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SmartObjectOnAnyOutput(object sender, EventArgs eventArgs)
		{
			RaiseOnAnyOutput();
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return Device != null && Device.IsOnline;
		}

		#endregion
	}
}
