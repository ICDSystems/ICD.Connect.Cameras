using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial
{
	public interface IRecentCallView : IView
	{
		/// <summary>
		/// Raised when the user presses the view.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Raised when the user presses the favorite button.
		/// </summary>
		event EventHandler OnFavoriteButtonPressed;

		/// <summary>
		/// Sets the user icon.
		/// </summary>
		/// <param name="icon"></param>
		void SetUserIcon(eRecentCallIconMode icon);

		/// <summary>
		/// Sets the contact name text.
		/// </summary>
		/// <param name="name"></param>
		void SetName(string name);

		/// <summary>
		/// Sets the contact detail text.
		/// </summary>
		/// <param name="details"></param>
		void SetDetailsText(string details);

		/// <summary>
		/// Sets the favorite state of the contact.
		/// </summary>
		/// <param name="favorite"></param>
		void SetFavorite(bool favorite);

		/// <summary>
		/// Sets the selected state of the view.
		/// </summary>
		/// <param name="selected"></param>
		void SetSelected(bool selected);
	}
}
