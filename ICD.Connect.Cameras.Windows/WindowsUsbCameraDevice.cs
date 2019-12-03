using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices.Windows;
using ICD.Connect.Settings;

namespace ICD.Connect.Cameras.Windows
{
	public sealed class WindowsUsbCameraDevice : AbstractCameraDevice<WindowsUsbCameraDeviceSettings>
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
