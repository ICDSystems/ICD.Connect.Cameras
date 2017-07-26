using System;
using System.Collections.Generic;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial
{
	public interface IDirectoryView : IView
	{
		/// <summary>
		/// Raised when the user presses the home button.
		/// </summary>
		event EventHandler OnHomeButtonPressed;

		/// <summary>
		/// Raised when the user presses the up button.
		/// </summary>
		event EventHandler OnUpButtonPressed;

		/// <summary>
		/// Raised when the user presses the dial button.
		/// </summary>
		event EventHandler OnDialButtonPressed;

		/// <summary>
		/// Sets the call type text on the dial button (i.e. Audio, Video)
		/// </summary>
		/// <param name="type"></param>
		void SetCallTypeLabel(string type);

		/// <summary>
		/// Sets the visibility of the home button.
		/// </summary>
		/// <param name="show"></param>
		void ShowHomeButton(bool show);

		/// <summary>
		/// Sets the visibility of the up button.
		/// </summary>
		/// <param name="show"></param>
		void ShowUpButton(bool show);

		/// <summary>
		/// Sets the enabled state of the dial button.
		/// </summary>
		/// <param name="enabled"></param>
		void EnableDialButton(bool enabled);

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IFavoritesAndDirectoryComponentView> GetChildCallViews(IViewFactory factory, ushort count);
	}
}
