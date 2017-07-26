using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings
{
	public interface ISettingsRoomInfoView : IView
	{
		/// <summary>
		/// Raised when the user presses a button.
		/// </summary>
		event EventHandler<UShortEventArgs> OnButtonPressed;

		/// <summary>
		/// Sets the button labels.
		/// </summary>
		/// <param name="labels"></param>
		void SetButtonLabels(IEnumerable<string> labels);
	}
}
