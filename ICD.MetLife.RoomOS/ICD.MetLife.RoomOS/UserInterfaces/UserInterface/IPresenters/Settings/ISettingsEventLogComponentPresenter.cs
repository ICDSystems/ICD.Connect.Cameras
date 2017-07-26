using ICD.Common.Services.Logging;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings
{
	public interface ISettingsEventLogComponentPresenter : IPresenter<ISettingsEventLogComponentView>
	{
		/// <summary>
		/// Gets/sets the log item.
		/// </summary>
		LogItem LogItem { get; set; }

		/// <summary>
		/// Gets/sets the item index.
		/// </summary>
		uint Index { get; set; }
	}
}
