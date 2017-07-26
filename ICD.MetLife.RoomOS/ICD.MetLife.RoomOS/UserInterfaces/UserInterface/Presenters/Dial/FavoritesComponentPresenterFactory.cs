using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;
using ICD.Connect.Conferencing.Favorites;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public sealed class FavoritesComponentPresenterFactory :
		AbstractListItemFactory<Favorite, IFavoritesComponentPresenter, IFavoritesAndDirectoryComponentView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public FavoritesComponentPresenterFactory(INavigationController navigationController,
		                                          ListItemFactory<IFavoritesAndDirectoryComponentView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(Favorite model, IFavoritesComponentPresenter presenter,
		                                     IFavoritesAndDirectoryComponentView view)
		{
			presenter.SetView(view);
			presenter.Favorite = model;
		}
	}
}
