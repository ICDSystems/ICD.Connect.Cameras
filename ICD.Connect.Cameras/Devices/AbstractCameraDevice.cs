using ICD.Connect.Devices;

namespace ICD.Connect.Cameras.Devices
{
	public abstract class AbstractCameraDevice<TSettings> : AbstractDevice<TSettings>, ICameraDevice
		where TSettings : ICameraDeviceSettings, new()
	{
	}
}
