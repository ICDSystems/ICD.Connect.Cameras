using ICD.Connect.Devices.Controls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Volume
{
	public interface IVolumeSidePresenter : IPresenter
	{
		/// <summary>
		/// Gets/sets the volume control.
		/// </summary>
		IVolumeDeviceControl VolumeControl { get; set; }

		/// <summary>
		/// Begins ramping the device volume up.
		/// </summary>
		void VolumeUp();

		/// <summary>
		/// Begins ramping the device volume down.
		/// </summary>
		void VolumeDown();

		/// <summary>
		/// Stops ramping the device volume.
		/// </summary>
		void Release();
	}
}
