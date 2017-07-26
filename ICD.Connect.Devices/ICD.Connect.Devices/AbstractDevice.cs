using ICD.Connect.Settings;

namespace ICD.Connect.Devices
{
	/// <summary>
	/// Base class for devices.
	/// </summary>
	public abstract class AbstractDevice<T> : AbstractDeviceBase<T>, IDevice
		where T : IDeviceSettings, new()
	{
	}
}
