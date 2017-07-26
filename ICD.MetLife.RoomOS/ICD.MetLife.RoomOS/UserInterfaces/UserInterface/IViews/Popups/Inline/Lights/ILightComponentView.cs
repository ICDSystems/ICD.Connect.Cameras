using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights
{
	public interface ILightComponentView : IView
	{
		/// <summary>
		/// Raised when the user presses the up button.
		/// </summary>
		event EventHandler OnUpButtonPressed;

		/// <summary>
		/// Raised when the user presses the down button.
		/// </summary>
		event EventHandler OnDownButtonPressed;

		/// <summary>
		/// Raised when the user releases a button.
		/// </summary>
		event EventHandler OnButtonReleased;

		/// <summary>
		/// Sets the value on the bar in the range 0.0f - 1.0f
		/// </summary>
		/// <param name="level"></param>
		void SetPercentage(float level);

		/// <summary>
		/// Sets the title text.
		/// </summary>
		/// <param name="title"></param>
		void SetTitle(string title);
	}
}
