using ICD.Connect.Settings;

namespace ICD.Connect.UI
{
	public interface IUserInterfaceFactory : IOriginator
	{
		/// <summary>
		/// Clears the instantiated user interfaces.
		/// </summary>
		void ClearUserInterfaces();

		/// <summary>
		/// Clears and rebuilds the user interfaces.
		/// </summary>
		void BuildUserInterfaces();
	}
}
