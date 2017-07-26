using System.Collections.Generic;

namespace ICD.Connect.Settings
{
	/// <summary>
	/// Base class for panel settings.
	/// </summary>
	public abstract class AbstractPanelDeviceSettings : AbstractDeviceBaseSettings, IPanelDeviceSettings
	{
		public const string PANEL_ELEMENT = "Panel";

		/// <summary>
		/// Gets the xml element.
		/// </summary>
		protected override string Element { get { return PANEL_ELEMENT; } }

		/// <summary>
		/// Returns the collection of ids that the settings will depend on.
		/// For example, to instantiate an IR Port from settings, the device the physical port
		/// belongs to will need to be instantiated first.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<int> GetDeviceDependencies()
		{
			// Typically a panel doesn't depend directly on a device.
			yield break;
		}
	}
}
