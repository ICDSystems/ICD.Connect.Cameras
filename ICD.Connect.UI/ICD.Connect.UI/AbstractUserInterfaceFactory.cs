using ICD.Connect.Settings;

namespace ICD.Connect.UI
{
	public abstract class AbstractUserInterfaceFactory<TSettings> : AbstractOriginator<TSettings>, IUserInterfaceFactory
		where TSettings : AbstractUserInterfaceFactorySettings, new()
	{
		/// <summary>
		/// Clears the instantiated user interfaces.
		/// </summary>
		public abstract void ClearUserInterfaces();

		/// <summary>
		/// Clears and rebuilds the user interfaces.
		/// </summary>
		public abstract void BuildUserInterfaces();
	}
}
