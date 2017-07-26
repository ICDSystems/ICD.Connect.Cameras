using System;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking
{
	public interface IPopupListView : IView
	{
		/// <summary>
		/// Raised when the user presses the close button.
		/// </summary>
		event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Raised when the user presses an item button in the button list.
		/// </summary>
		event EventHandler<UShortEventArgs> OnItemButtonPressed;

		/// <summary>
		/// Sets the label for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		void SetItemLabel(ushort index, string label);

		/// <summary>
		/// Sets the selection state for the item at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetItemSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the title text for the popup.
		/// </summary>
		/// <param name="title"></param>
		void SetTitle(string title);

		/// <summary>
		/// Sets the number of items in the button list.
		/// </summary>
		/// <param name="count"></param>
		void SetItemCount(ushort count);
	}
}
