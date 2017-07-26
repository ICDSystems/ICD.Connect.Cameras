using ICD.Connect.TvPresets;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.TvTuner
{
	public sealed class ChannelPresetPresenterFactory :
		AbstractListItemFactory<Station, IChannelPresetPresenter, IChannelPresetView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public ChannelPresetPresenterFactory(INavigationController navigationController,
		                                     ListItemFactory<IChannelPresetView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(Station model, IChannelPresetPresenter presenter, IChannelPresetView view)
		{
			presenter.SetView(view);
			presenter.Station = model;
		}
	}
}
