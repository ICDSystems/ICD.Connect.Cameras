using ICD.Connect.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsDeviceListComponentPresenterFactory :
		AbstractListItemFactory<ISettings, ISettingsDeviceListComponentPresenter, ISettingsDeviceListComponentView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public SettingsDeviceListComponentPresenterFactory(INavigationController navigationController,
		                                                   ListItemFactory<ISettingsDeviceListComponentView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(ISettings model, ISettingsDeviceListComponentPresenter presenter,
		                                     ISettingsDeviceListComponentView view)
		{
			presenter.SetView(view);
			presenter.Settings = model;
		}
	}
}
