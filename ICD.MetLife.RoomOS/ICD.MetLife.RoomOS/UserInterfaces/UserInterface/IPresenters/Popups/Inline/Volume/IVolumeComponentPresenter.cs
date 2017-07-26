using System;
using ICD.Connect.Devices.Controls;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Volume;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Volume
{
	public interface IVolumeComponentPresenter : IPresenter<IVolumeComponentView>
	{
		/// <summary>
		/// Raised when the user presses the volume up button.
		/// </summary>
		event EventHandler OnVolumeUpButtonPressed;

		/// <summary>
		/// Raised when the user presses the volume down button.
		/// </summary>
		event EventHandler OnVolumeDownButtonPressed;

		/// <summary>
		/// Raised when the user releases a volume button.
		/// </summary>
		event EventHandler OnVolumeButtonReleased;

		/// <summary>
		/// Raised when the user presses the mute button.
		/// </summary>
		event EventHandler OnMuteButtonPressed;

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
