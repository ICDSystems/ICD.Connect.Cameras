﻿using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Volume
{
	public interface IVolumeSideView : IView
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
		/// Sets the value on the volume bar in the range 0.0f - 1.0f
		/// </summary>
		/// <param name="volume"></param>
		void SetVolumePercentage(float volume);

		/// <summary>
		/// Sets the title text.
		/// </summary>
		/// <param name="title"></param>
		void SetTitle(string title);

		/// <summary>
		/// Sets the enabled state of the volume guage.
		/// </summary>
		/// <param name="enabled"></param>
		void SetGuageEnabled(bool enabled);
	}
}
