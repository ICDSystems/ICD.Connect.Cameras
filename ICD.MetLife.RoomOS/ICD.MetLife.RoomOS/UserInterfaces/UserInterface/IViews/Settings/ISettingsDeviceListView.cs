using System;
using System.Collections.Generic;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings
{
	public interface ISettingsDeviceListView : IView
	{
		/// <summary>
		/// Raised when the user presses the exit button.
		/// </summary>
		event EventHandler OnExitButtonPressed;

		/// <summary>
		/// Sets the title label text.
		/// </summary>
		/// <param name="title"></param>
		void SetTitle(string title);

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<ISettingsDeviceListComponentView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
