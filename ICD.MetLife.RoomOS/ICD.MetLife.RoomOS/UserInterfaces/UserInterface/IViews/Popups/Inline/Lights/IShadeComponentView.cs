using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights
{
	public interface IShadeComponentView : IView
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
		/// Raised when the user presses the stop button.
		/// </summary>
		event EventHandler OnStopButtonPressed;

		/// <summary>
		/// Sets the title text.
		/// </summary>
		/// <param name="title"></param>
		void SetTitle(string title);
	}
}
