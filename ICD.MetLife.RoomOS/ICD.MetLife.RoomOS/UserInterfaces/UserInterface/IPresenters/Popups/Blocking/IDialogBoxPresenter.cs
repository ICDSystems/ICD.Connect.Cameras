using System.Collections.Generic;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking
{
	public interface IDialogBoxPresenter : IPresenter
	{
		/// <summary>
		/// Sets the dialog message and callbacks.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="buttons"></param>
		void SetDialog(string message, IEnumerable<DialogBoxButton> buttons);
	}
}
