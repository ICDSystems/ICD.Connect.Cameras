using ICD.Connect.Devices;
using ICD.Connect.Settings;

namespace ICD.Connect.Panels
{
	public abstract class AbstractSigDevice<TSettings> : AbstractSigDeviceBase<TSettings>, IDevice
		where TSettings : AbstractDeviceSettings, new()
	{
	}
}
