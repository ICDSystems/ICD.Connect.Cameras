using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking.Keyboard;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking.Keyboard
{
	public interface IPopupKeyboardAlphaPresenter : IPresenter
	{
		/// <summary>
		/// Raised when the user presses a key.
		/// </summary>
		event PopupKeyboardKeyPressedCallback OnKeyPressed;

		/// <summary>
		/// Gets/sets the shift state.
		/// </summary>
		bool Shift { get; set; }

		/// <summary>
		/// Gets/sets the caps state.
		/// </summary>
		bool Caps { get; set; }
	}
}
