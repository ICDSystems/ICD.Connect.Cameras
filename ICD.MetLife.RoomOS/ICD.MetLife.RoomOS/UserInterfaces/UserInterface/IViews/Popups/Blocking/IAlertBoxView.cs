using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking
{
	public interface IAlertBoxView : IView
	{
		/// <summary>
		/// Raised when the user presses a button.
		/// </summary>
		event EventHandler<UShortEventArgs> OnButtonPressed;

		/// <summary>
		/// Sets the title label text.
		/// </summary>
		/// <param name="title"></param>
		void SetMessage(string title);

		/// <summary>
		/// Sets the button labels.
		/// </summary>
		/// <param name="labels"></param>
		void SetButtonLabels(IEnumerable<string> labels);
	}
}
