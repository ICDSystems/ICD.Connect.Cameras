namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings
{
	public enum eSettingsType
	{
		Devices,
		Ports,
		Panels,
		Connections,
		Sources,
		Destinations
	}

	public interface ISettingsDeviceListPresenter : IPresenter
	{
		/// <summary>
		/// Gets/sets the current settings type.
		/// </summary>
		eSettingsType Mode { get; set; }
	}
}
