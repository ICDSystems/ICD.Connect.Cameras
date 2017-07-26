using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings
{
	public interface ISettingsDeviceListComponentView : IView
	{
		/// <summary>
		/// Raised when the user presses the component.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Raised when the user presses the delete button.
		/// </summary>
		event EventHandler OnDeleteButtonPressed;

		/// <summary>
		/// Sets the label text for the component.
		/// </summary>
		/// <param name="label"></param>
		void SetLabel(string label);

		/// <summary>
		/// Sets the visibility of the delete button.
		/// </summary>
		/// <param name="show"></param>
		void ShowDeleteButton(bool show);
	}
}
