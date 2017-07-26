using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial
{
	public interface IFavoritesAndDirectoryComponentView : IView
	{
		/// <summary>
		/// Raised when the view is pressed.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Raised when the favorite button is pressed.
		/// </summary>
		event EventHandler OnFavoriteButtonPressed;

		/// <summary>
		/// Sets the icon for the contact.
		/// </summary>
		/// <param name="icon"></param>
		void SetIcon(eRecentCallIconMode icon);

		/// <summary>
		/// Sets the name of the contact.
		/// </summary>
		/// <param name="name"></param>
		void SetName(string name);

		/// <summary>
		/// Sets the favorite state of the contact.
		/// </summary>
		/// <param name="favorite"></param>
		void SetFavorite(bool favorite);

		/// <summary>
		/// Sets the visibility of the favorite button.
		/// </summary>
		/// <param name="show"></param>
		void ShowFavoriteButton(bool show);

		/// <summary>
		/// Sets the selected state of the component.
		/// </summary>
		/// <param name="selected"></param>
		void SetSelected(bool selected);
	}
}
