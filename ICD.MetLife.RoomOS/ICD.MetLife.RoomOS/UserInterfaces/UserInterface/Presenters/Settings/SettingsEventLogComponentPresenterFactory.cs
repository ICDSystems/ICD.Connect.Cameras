using System.Collections.Generic;
using ICD.Common.Services.Logging;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsEventLogComponentPresenterFactory :
		AbstractListItemFactory
			<KeyValuePair<int, LogItem>, ISettingsEventLogComponentPresenter, ISettingsEventLogComponentView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public SettingsEventLogComponentPresenterFactory(INavigationController navigationController,
		                                                 ListItemFactory<ISettingsEventLogComponentView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(KeyValuePair<int, LogItem> model, ISettingsEventLogComponentPresenter presenter,
		                                     ISettingsEventLogComponentView view)
		{
			presenter.SetView(view);
			presenter.Index = (uint)model.Key;
			presenter.LogItem = model.Value;
		}
	}
}
