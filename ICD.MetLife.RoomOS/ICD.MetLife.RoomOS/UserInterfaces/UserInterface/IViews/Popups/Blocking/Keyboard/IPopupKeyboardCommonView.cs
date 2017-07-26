using System;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking.Keyboard
{
	public delegate void PopupKeyboardKeyPressedCallback(object sender, KeyboardKey key);

	public interface IPopupKeyboardCommonView : IView
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
		/// Raised when the user presses the submit button.
		/// </summary>
		event EventHandler OnSubmitButtonPressed;

		/// <summary>
		/// Raised when the user presses the cancel button.
		/// </summary>
		event EventHandler OnCancelButtonPressed;

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
		/// Sets the label text on the submit button.
		/// </summary>
		/// <param name="text"></param>
		void SetSubmitButtonLabel(string text);

		/// <summary>
		/// Sets the label text on the cancel button.
		/// </summary>
		/// <param name="text"></param>
		void SetCancelButtonLabel(string text);

		/// <summary>
		/// Sets the enabled state of the submit button.
		/// </summary>
		/// <param name="enable"></param>
		void EnableSubmitButton(bool enable);

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
