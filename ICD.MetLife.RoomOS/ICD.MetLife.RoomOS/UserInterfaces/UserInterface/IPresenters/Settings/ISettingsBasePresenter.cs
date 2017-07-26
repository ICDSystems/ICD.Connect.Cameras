using ICD.Connect.Settings.Core;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings
{
	public interface ISettingsBasePresenter : IPresenter
	{
		/// <summary>
		/// We store the user configured settings in this property until the user wishes to save.
		/// </summary>
		ICoreSettings SettingsInstance { get; }

		/// <summary>
		/// Updates the SettingsInstance property with the current state of the core.
		/// </summary>
		void RevertSettingsInstance();
	}
}
