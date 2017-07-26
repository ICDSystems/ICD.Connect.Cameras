using System;
using System.Collections.Generic;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial
{
	public interface IRecentsView : IView
	{
		/// <summary>
		/// Raised when the user presses the remove from list button.
		/// </summary>
		event EventHandler OnRemoveFromListButtonPressed;

		/// <summary>
		/// Raised when the user presses the dial button.
		/// </summary>
		event EventHandler OnDialButtonPressed;

		/// <summary>
		/// Sets the call type text on the dial button (i.e. Audio, Video)
		/// </summary>
		/// <param name="type"></param>
		void SetCallTypeText(string type);

		/// <summary>
		/// Sets the visibility of the remove from list button.
		/// </summary>
		/// <param name="show"></param>
		void ShowRemoveFromListButton(bool show);

		/// <summary>
		/// Sets the enabled state of the dial button.
		/// </summary>
		/// <param name="enabled"></param>
		void EnabledDialButton(bool enabled);

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IRecentCallView> GetChildCallViews(IViewFactory factory, ushort count);
	}
}
