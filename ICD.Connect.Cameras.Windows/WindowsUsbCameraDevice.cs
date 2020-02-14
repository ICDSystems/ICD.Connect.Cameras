using System;
using System.Collections.Generic;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices.Windows;
using ICD.Connect.Settings;

namespace ICD.Connect.Cameras.Windows
{
	public sealed class WindowsUsbCameraDevice : AbstractCameraDevice<WindowsUsbCameraDeviceSettings>, IWindowsDevice
	{
		/// <summary>
		/// Gets the path to the device on windows.
		/// </summary>
		public WindowsDevicePathInfo DevicePath { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public WindowsUsbCameraDevice()
		{
			Controls.Add(new GenericCameraRouteSourceControl<WindowsUsbCameraDevice>(this, 0));
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return true;
		}

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			DevicePath = default(WindowsDevicePathInfo);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(WindowsUsbCameraDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.DevicePath = DevicePath;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(WindowsUsbCameraDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			DevicePath = settings.DevicePath;
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the maximum number of presets this camera can support
		/// </summary>
		public override int MaxPresets { get { return 0; } }

		/// <summary>
		/// Begins panning the camera
		/// </summary>
		/// <param name="action"></param>
		public override void Pan(eCameraPanAction action)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Begin tilting the camera.
		/// </summary>
		public override void Tilt(eCameraTiltAction action)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Zooms the camera.
		/// </summary>
		public override void Zoom(eCameraZoomAction action)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the stored camera presets.
		/// </summary>
		public override IEnumerable<CameraPreset> GetPresets()
		{
			yield break;
		}

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		public override void ActivatePreset(int presetId)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Stores the cameras current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		public override void StorePreset(int presetId)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Sets if the camera mute state should be active
		/// </summary>
		/// <param name="enable"></param>
		public override void MuteCamera(bool enable)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Resets camera to its predefined home position
		/// </summary>
		public override void SendCameraHome()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Device Path", DevicePath);
		}

		#endregion
	}
}
