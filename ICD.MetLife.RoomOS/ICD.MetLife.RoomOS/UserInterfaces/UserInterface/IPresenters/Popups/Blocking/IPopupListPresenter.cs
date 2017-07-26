using System;
using System.Collections.Generic;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking
{
	public interface IPopupListPresenter : IPresenter
	{
		/// <summary>
		/// Clears the list.
		/// </summary>
		void Clear();

		/// <summary>
		/// Sets the selection state of the given item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="selected"></param>
		void SetItemSelected(object item, bool selected);

		/// <summary>
		/// Returns true if the given item is selected.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool GetItemSelected(object item);

		/// <summary>
		/// Returns the items that are selected.
		/// </summary>
		/// <returns></returns>
		IEnumerable<object> GetSelectedItems();

		/// <summary>
		/// Populates the list.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="getLabel"></param>
		/// <param name="getSelected"></param>
		/// <param name="title"></param>
		/// <param name="itemPressedCallback"></param>
		/// <param name="closedCallback"></param>
		void SetListItems(IEnumerable<object> items, Func<object, string> getLabel, Func<object, bool> getSelected,
		                  string title, Action<object> itemPressedCallback, Action closedCallback);
	}

	/// <summary>
	/// Extension methods for the IPopupListPresenter.
	/// </summary>
	public static class PopupListPresenterExtensions
	{
		/// <summary>
		/// Populates the list.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="items"></param>
		/// <param name="getLabel"></param>
		/// <param name="title"></param>
		/// <param name="itemPressedCallback"></param>
		public static void SetListItems(this IPopupListPresenter extends, IEnumerable<object> items,
		                                Func<object, string> getLabel, string title, Action<object> itemPressedCallback)
		{
			extends.SetListItems(items, getLabel, i => false, title, itemPressedCallback, () => { });
		}
	}
}
