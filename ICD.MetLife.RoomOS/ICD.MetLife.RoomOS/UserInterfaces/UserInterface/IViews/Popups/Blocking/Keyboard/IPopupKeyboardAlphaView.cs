using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking.Keyboard
{
	public interface IPopupKeyboardAlphaView : IView
	{
		/// <summary>
		/// Raised when the user presses the special button.
		/// </summary>
		event EventHandler OnSpecialButtonPressed;

		/// <summary>
		/// Raised when the user presses a key button.
		/// </summary>
		event PopupKeyboardKeyPressedCallback OnKeyPressed;

		/// <summary>
		/// Sets the shift state of the chars.
		/// </summary>
		/// <param name="shift"></param>
		/// <param name="caps"></param>
		void SetShift(bool shift, bool caps);
	}
}
