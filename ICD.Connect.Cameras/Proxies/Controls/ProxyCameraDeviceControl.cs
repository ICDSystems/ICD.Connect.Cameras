using ICD.Connect.Devices.Proxies.Devices;

namespace ICD.Connect.Cameras.Proxies.Controls
{
	public sealed class ProxyCameraDeviceControl : AbstractProxyCameraDeviceControl
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public ProxyCameraDeviceControl(IProxyDeviceBase parent, int id)
			: base(parent, id)
		{
		}
	}
}
