using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Common
{
	/// <summary>
	/// Represents the bar along the top of the panel.
	/// </summary>
	public interface IStatusBarView : IView
	{
		/// <summary>
		/// Raised when the user presses the audio button.
		/// </summary>
		event EventHandler OnAudioButtonPressed;

		/// <summary>
		/// Raised when the user presses the settings button.
		/// </summary>
		event EventHandler OnSettingsButtonPressed;

		/// <summary>
		/// Sets the room name text.
		/// </summary>
		/// <param name="text"></param>
		void SetRoomNameText(string text);

		/// <summary>
		/// Sets the room prefix text.
		/// </summary>
		/// <param name="text"></param>
		void SetRoomPrefixText(string text);

		/// <summary>
		/// Sets the color of the status bar.
		/// </summary>
		/// <param name="color"></param>
		void SetColor(eColor color);
	}
}
