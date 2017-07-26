using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial
{
	public interface IKeyboardSpecialView : IView
	{
		/// <summary>
		/// Raised when the user presses the alphabet button.
		/// </summary>
		event EventHandler OnAlphabetButtonPressed;

		/// <summary>
		/// Raised when the user presses a key button.
		/// </summary>
		event KeyboardKeyPressedCallback OnKeyPressed;

		/// <summary>
		/// Sets the shift state of the special chars.
		/// </summary>
		/// <param name="shift"></param>
		void SetShift(bool shift);
	}
}
