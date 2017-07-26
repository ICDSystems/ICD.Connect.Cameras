using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings.SettingsDevicePropertiesComponents
{
	public interface ISettingsDevicePropertiesComponentPresenter : IPresenter<ISettingsDevicePropertiesComponentView>
	{
		/// <summary>
		/// Sets the property.
		/// </summary>
		/// <param name="model"></param>
		void SetModel(PropertySettingsPair model);
	}
}
