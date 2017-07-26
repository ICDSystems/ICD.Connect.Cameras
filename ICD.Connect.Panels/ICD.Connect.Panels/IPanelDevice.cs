using ICD.Common.Properties;
using ICD.Connect.Panels.SmartObjectCollections;

namespace ICD.Connect.Panels
{
	public interface IPanelDevice : ISigDevice
	{
		/// <summary>
		/// Collection containing the loaded SmartObjects of this device.
		/// </summary>
		[PublicAPI]
		ISmartObjectCollection SmartObjects { get; }
	}
}
