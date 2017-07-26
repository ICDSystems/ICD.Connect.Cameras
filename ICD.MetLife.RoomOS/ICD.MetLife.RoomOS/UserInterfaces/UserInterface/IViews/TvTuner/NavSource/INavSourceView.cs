using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner.NavSource
{
	public interface INavSourceView : IView
	{
		/// <summary>
		/// Raised when the user presses the stop button.
		/// </summary>
		event EventHandler OnStopButtonPressed;

		/// <summary>
		/// Sets the enabled state of the stop button.
		/// </summary>
		/// <param name="enabled"></param>
		void EnableStopButton(bool enabled);
	}
}
