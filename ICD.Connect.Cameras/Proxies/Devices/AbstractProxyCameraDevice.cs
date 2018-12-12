using ICD.Connect.Devices.Proxies.Devices;

namespace ICD.Connect.Cameras.Proxies.Devices
{
	public abstract class AbstractProxyCameraDevice<TSettings> : AbstractProxyDevice<TSettings>, IProxyCameraDevice
		where TSettings : IProxyCameraDeviceSettings
	{
	}
}
