using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings
{
	public interface ISettingsBaseView : IView
	{
		/// <summary>
		/// Raised when the user presses one of the items in the button list.
		/// </summary>
		event EventHandler<UShortEventArgs> OnItemPressed;

		/// <summary>
		/// Raised when the user presses the exit button.
		/// </summary>
		event EventHandler OnExitButtonPressed;

		/// <summary>
		/// Sets the button labels.
		/// </summary>
		/// <param name="labels"></param>
		void SetButtonLabels(IEnumerable<string> labels);
	}
}
