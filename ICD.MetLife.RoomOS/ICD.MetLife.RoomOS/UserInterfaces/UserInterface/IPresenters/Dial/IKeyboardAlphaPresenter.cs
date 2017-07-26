using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial
{
	public interface IKeyboardAlphaPresenter : IPresenter
	{
		/// <summary>
		/// Raised when the user presses a key.
		/// </summary>
		event KeyboardKeyPressedCallback OnKeyPressed;

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
