using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;
using ICD.Connect.Conferencing.ConferenceSources;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public sealed class RecentsComponentPresenterFactory :
		AbstractListItemFactory<IConferenceSource, IRecentCallPresenter, IRecentCallView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public RecentsComponentPresenterFactory(INavigationController navigationController,
		                                        ListItemFactory<IRecentCallView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IConferenceSource model, IRecentCallPresenter presenter, IRecentCallView view)
		{
			presenter.SetView(view);
			presenter.Source = model;
		}
	}
}
