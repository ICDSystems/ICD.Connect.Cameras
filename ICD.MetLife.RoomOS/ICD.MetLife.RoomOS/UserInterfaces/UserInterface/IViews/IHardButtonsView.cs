﻿using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews
{
	/// <summary>
	/// Represents the physical buttons on the user input device.
	/// </summary>
	public interface IHardButtonsView : IView
	{
		/// <summary>
		/// Raised when the user presses the power button.
		/// </summary>
		event EventHandler OnPowerButtonPressed;

		/// <summary>
		/// Raised when the user presses the home button.
		/// </summary>
		event EventHandler OnHomeButtonPressed;

		/// <summary>
		/// Raised when the user presses the lights button.
		/// </summary>
		event EventHandler OnLightsButtonPressed;

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
	}
}
