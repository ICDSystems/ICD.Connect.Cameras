using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Layout
{
	public interface ILayoutView : IView
	{
		/// <summary>
		/// Raised when the user presses the auto layout button.
		/// </summary>
		event EventHandler OnAutoButtonPressed;

		/// <summary>
		/// Raised when the user presses the equal layout button.
		/// </summary>
		event EventHandler OnEqualButtonPressed;

		/// <summary>
		/// Raised when the user presses the prominent layout button.
		/// </summary>
		event EventHandler OnProminentButtonPressed;

		/// <summary>
		/// Raised when the user presses the overlay layout button.
		/// </summary>
		event EventHandler OnOverlayButtonPressed;

		/// <summary>
		/// Raised when the user presses the single layout button.
		/// </summary>
		event EventHandler OnSingleButtonPressed;
	}
}
