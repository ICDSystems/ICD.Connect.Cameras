using System;
using ICD.Connect.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings
{
	public interface ISettingsDeviceListComponentPresenter : IPresenter<ISettingsDeviceListComponentView>
	{
		/// <summary>
		/// Raised when the delete button is pressed.
		/// </summary>
		event EventHandler OnDeleteButtonPressed;

		/// <summary>
		/// Raised when the item button is pressed.
		/// </summary>
		event EventHandler OnItemButtonPressed;

		/// <summary>
		/// Gets/sets the settings for this item.
		/// </summary>
		ISettings Settings { get; set; }

		/// <summary>
		/// Gets the title label for this property.
		/// </summary>
		string Title { get; }
	}
}
