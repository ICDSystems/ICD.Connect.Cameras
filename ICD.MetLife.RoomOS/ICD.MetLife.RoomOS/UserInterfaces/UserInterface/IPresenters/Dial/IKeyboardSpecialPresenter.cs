using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial
{
	public interface IKeyboardSpecialPresenter : IPresenter
	{
		/// <summary>
		/// Raised when the user presses a key.
		/// </summary>
		event KeyboardKeyPressedCallback OnKeyPressed;

		/// <summary>
		/// Gets/sets the shift state.
		/// </summary>
		bool Shift { get; set; }
	}
}
