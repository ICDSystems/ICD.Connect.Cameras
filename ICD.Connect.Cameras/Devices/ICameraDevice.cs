using ICD.Connect.API.Attributes;
using ICD.Connect.Cameras.Proxies.Devices;
using ICD.Connect.Devices;

namespace ICD.Connect.Cameras.Devices
{
	[ApiClass(typeof(ProxyCameraDevice), typeof(IDevice))]
	public interface ICameraDevice : IDevice
	{
	}
}

