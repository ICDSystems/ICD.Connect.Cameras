using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home;
using ICD.Connect.Conferencing.ConferenceSources;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Home
{
	public sealed class CallStatusComponentPresenterFactory :
		AbstractListItemFactory<IConferenceSource, ICallStatusPresenter, ICallStatusView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public CallStatusComponentPresenterFactory(INavigationController navigationController,
		                                           ListItemFactory<ICallStatusView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IConferenceSource model, ICallStatusPresenter presenter, ICallStatusView view)
		{
			presenter.SetView(view);
			presenter.ConferenceSource = model;
		}
	}
}
