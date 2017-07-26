using System;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial
{
	public interface IDialpadView : IView
	{
		/// <summary>
		/// Raised when the user presses the dial button.
		/// </summary>
		event EventHandler OnDialButtonPressed;

		/// <summary>
		/// Raised when the user presses the backspace button.
		/// </summary>
		event EventHandler OnBackspaceButtonPressed;

		/// <summary>
		/// Raised when the user presses the clear button.
		/// </summary>
		event EventHandler OnClearButtonPressed;

		/// <summary>
		/// Raised when the user presses a keypad button.
		/// </summary>
		event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		/// <summary>
		/// Raised when the user enters text in the text field.
		/// </summary>
		event EventHandler<StringEventArgs> OnTextEntryModified;

		/// <summary>
		/// Sets the call type text in the dial button (i.e. Audio, Video)
		/// </summary>
		/// <param name="type"></param>
		void SetCallTypeLabel(string type);

		/// <summary>
		/// Sets the text in the text entry field.
		/// </summary>
		/// <param name="text"></param>
		void SetTextEntryText(string text);

		/// <summary>
		/// Sets the enable state of the clear button.
		/// </summary>
		/// <param name="enable"></param>
		void EnableClearButton(bool enable);

		/// <summary>
		/// Sets the enable state of the backspace button.
		/// </summary>
		/// <param name="enable"></param>
		void EnableBackspaceButton(bool enable);

		/// <summary>
		/// Enables/disables the dial button.
		/// </summary>
		/// <param name="enabled"></param>
		void EnableDialButton(bool enabled);
	}
}
