using ICD.Connect.API.Attributes;
using ICD.Connect.Cameras.Proxies.Controls;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Cameras.Controls
{
	[ApiClass(typeof(ProxyCameraDeviceControl), typeof(IDeviceControl))]
	public interface ICameraDeviceControl : IDeviceControl
	{
	}
}
