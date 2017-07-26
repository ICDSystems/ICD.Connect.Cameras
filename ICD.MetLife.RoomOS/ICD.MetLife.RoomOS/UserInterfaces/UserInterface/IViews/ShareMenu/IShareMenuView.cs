using System;
using System.Collections.Generic;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.ShareMenu
{
	public interface IShareMenuView : IView
	{
		/// <summary>
		/// Raised when the user presses the stop sharing button.
		/// </summary>
		event EventHandler OnStopSharingButtonPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IShareComponentView> GetChildCallViews(IViewFactory factory, ushort count);

		/// <summary>
		/// Sets the enabled state of the stop sharing button.
		/// </summary>
		/// <param name="enable"></param>
		void EnableStopSharingButton(bool enable);
	}
}
