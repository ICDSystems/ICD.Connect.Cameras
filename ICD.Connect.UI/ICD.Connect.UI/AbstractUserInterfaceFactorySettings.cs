using System.Collections.Generic;
using ICD.Connect.Settings;

namespace ICD.Connect.UI
{
	public abstract class AbstractUserInterfaceFactorySettings : AbstractSettings, IUserInterfaceFactorySettings
	{
		private const string USER_INTERFACE_FACTORY_ELEMENT = "UserInterfaceFactory";

		/// <summary>
		/// Gets the xml element.
		/// </summary>
		protected override string Element { get { return USER_INTERFACE_FACTORY_ELEMENT; } }

		/// <summary>
		/// Returns the collection of ids that the settings will depend on.
		/// For example, to instantiate an IR Port from settings, the device the physical port
		/// belongs to will need to be instantiated first.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<int> GetDeviceDependencies()
		{
			yield break;
		}
	}
}
