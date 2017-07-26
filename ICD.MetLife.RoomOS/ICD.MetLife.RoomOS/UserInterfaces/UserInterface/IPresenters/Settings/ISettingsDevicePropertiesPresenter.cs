using ICD.Connect.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings
{
	public interface ISettingsDevicePropertiesPresenter : IPresenter
	{
		/// <summary>
		/// Gets/sets the current settings instance.
		/// </summary>
		ISettings Settings { get; set; }
	}
}
