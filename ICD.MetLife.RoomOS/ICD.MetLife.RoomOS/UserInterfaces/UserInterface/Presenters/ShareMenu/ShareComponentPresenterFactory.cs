using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.ShareMenu;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.ShareMenu;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.ShareMenu
{
	public sealed class ShareComponentPresenterFactory :
		AbstractListItemFactory<MetlifeSource, IShareComponentPresenter, IShareComponentView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public ShareComponentPresenterFactory(INavigationController navigationController,
		                                      ListItemFactory<IShareComponentView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(MetlifeSource model, IShareComponentPresenter presenter,
		                                     IShareComponentView view)
		{
			presenter.SetView(view);
			presenter.Source = model;
		}
	}
}
