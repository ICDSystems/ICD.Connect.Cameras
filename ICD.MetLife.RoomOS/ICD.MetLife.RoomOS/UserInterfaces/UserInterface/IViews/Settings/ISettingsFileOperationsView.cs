using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings
{
	public interface ISettingsFileOperationsView : IView
	{
		/// <summary>
		/// Raised when the user presses the panel setup button.
		/// </summary>
		event EventHandler OnPanelSetupButtonPressed;

		/// <summary>
		/// Raised when the user presses the program reset button.
		/// </summary>
		event EventHandler OnProgramResetButtonPressed;

		/// <summary>
		/// Raised when the user presses the processor reset button.
		/// </summary>
		event EventHandler OnProcessorResetButtonPressed;

		/// <summary>
		/// Raised when the user presses the save and run button.
		/// </summary>
		event EventHandler OnSaveAndRunButtonPressed;

		/// <summary>
		/// Raised when the user presses the undo changes button.
		/// </summary>
		event EventHandler OnUndoButtonPressed;

		/// <summary>
		/// Raised when the user presses the load button.
		/// </summary>
		event EventHandler OnLoadButtonPressed;

		/// <summary>
		/// Sets the program information text.
		/// </summary>
		/// <param name="text"></param>
		void SetProgramInfoText(string text);
	}
}
