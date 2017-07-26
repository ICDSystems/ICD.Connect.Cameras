using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Settings;

namespace ICD.Connect.Devices
{
	public interface IDeviceBase : IOriginator, IStateDisposable
	{
		/// <summary>
		/// Raised when the device goes online/offline.
		/// </summary>
		[PublicAPI]
		event EventHandler<BoolEventArgs> OnIsOnlineStateChanged;

		/// <summary>
		/// Returns true if the device hardware is detected by the system.
		/// </summary>
		[PublicAPI]
		bool IsOnline { get; }

		/// <summary>
		/// Gets the controls for this device.
		/// </summary>
		[PublicAPI]
		DeviceControlsCollection Controls { get; }
	}
}
