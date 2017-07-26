using System;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline
{
	public interface ISettingsStandardView : IView
	{
		/// <summary>
		/// Raised when the user presses the enter button.
		/// </summary>
		event EventHandler OnEnterButtonPressed;

		/// <summary>
		/// Raised when the user presses the clear button.
		/// </summary>
		event EventHandler OnClearButtonPressed;

		/// <summary>
		/// Raised when the user presses a keypad button.
		/// </summary>
		event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		/// <summary>
		/// Sets the text in the password text field.
		/// </summary>
		/// <param name="password"></param>
		void SetPasswordText(string password);

		/// <summary>
		/// Sets the number of devices.
		/// </summary>
		/// <param name="count"></param>
		void SetDeviceCount(ushort count);

		/// <summary>
		/// Sets the label for the device at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="name"></param>
		/// <param name="color"></param>
		void SetDeviceLabel(ushort index, string name, eColor color);
	}
}
