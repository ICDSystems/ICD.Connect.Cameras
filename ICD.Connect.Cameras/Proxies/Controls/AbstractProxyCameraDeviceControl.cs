using ICD.Connect.Devices.Proxies.Controls;
using ICD.Connect.Devices.Proxies.Devices;

namespace ICD.Connect.Cameras.Proxies.Controls
{
	public abstract class AbstractProxyCameraDeviceControl : AbstractProxyDeviceControl, IProxyCameraDeviceControl
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		protected AbstractProxyCameraDeviceControl(IProxyDeviceBase parent, int id)
			: base(parent, id)
		{
		}
	}
}
