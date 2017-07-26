using System;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial
{
	public delegate void KeyboardKeyPressedCallback(object sender, KeyboardKey key);

	public interface IKeyboardCommonView : IView
	{
		/// <summary>
		/// Raised when the user enters text in the text field.
		/// </summary>
		event EventHandler<StringEventArgs> OnTextEntered;

		/// <summary>
		/// Raised when the user presses the backspace button.
		/// </summary>
		event EventHandler OnBackspaceButtonPressed;

		/// <summary>
		/// Raised when the user presses the clear button.
		/// </summary>
		event EventHandler OnClearButtonPressed;

		/// <summary>
		/// Raised when the user presses the space button.
		/// </summary>
		event EventHandler OnSpaceButtonPressed;

		/// <summary>
		/// Raised when the user presses the caps button.
		/// </summary>
		event EventHandler OnCapsButtonPressed;

		/// <summary>
		/// Raised when the user presses the shift button.
		/// </summary>
		event EventHandler OnShiftButtonPressed;

		/// <summary>
		/// Raised when the user presses the dial button.
		/// </summary>
		event EventHandler OnDialButtonPressed;

		/// <summary>
		/// Sets the selected state of the caps button.
		/// </summary>
		void SelectCapsButton(bool select);

		/// <summary>
		/// Sets the selected state of the shift button.
		/// </summary>
		void SelectShiftButton(bool select);

		/// <summary>
		/// Sets the text in the text entry field.
		/// </summary>
		/// <param name="text"></param>
		void SetText(string text);

		/// <summary>
		/// Sets the call type label on the dial button.
		/// </summary>
		/// <param name="text"></param>
		void SetDialTypeText(string text);

		/// <summary>
		/// Sets the enabled state of the dial button.
		/// </summary>
		/// <param name="enable"></param>
		void EnableDialButton(bool enable);

		/// <summary>
		/// Sets the enabled state of the backspace button.
		/// </summary>
		/// <param name="enable"></param>
		void EnableBackspaceButton(bool enable);

		/// <summary>
		/// Sets the enabled state of the clear button.
		/// </summary>
		/// <param name="enable"></param>
		void EnableClearButton(bool enable);
	}
}
