using System;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial
{
	public interface IFavoritesAndDirectoryComponentPresenter : IPresenter<IFavoritesAndDirectoryComponentView>
	{
		/// <summary>
		/// Raised when the user presses the component.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Gets/sets the component selection state.
		/// </summary>
		bool Selected { get; set; }
	}
}
