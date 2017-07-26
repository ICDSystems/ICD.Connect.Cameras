using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline
{
	public interface IPopupBaseView : IView
	{
		/// <summary>
		/// Raised when the user presses the close button
		/// </summary>
		event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Sets the title text.
		/// </summary>
		/// <param name="label"></param>
		void SetTitle(string label);
	}
}
