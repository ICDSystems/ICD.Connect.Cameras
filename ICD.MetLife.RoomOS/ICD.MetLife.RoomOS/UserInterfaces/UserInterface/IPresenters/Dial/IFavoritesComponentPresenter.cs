using System;
using ICD.Connect.Conferencing.Favorites;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial
{
	public interface IFavoritesComponentPresenter : IFavoritesAndDirectoryComponentPresenter
	{
		event EventHandler OnIsFavoriteStateChanged;

		Favorite Favorite { get; set; }
	}
}
