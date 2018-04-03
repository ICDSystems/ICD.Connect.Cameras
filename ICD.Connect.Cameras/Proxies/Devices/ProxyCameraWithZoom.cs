using ICD.Connect.Cameras.Devices;

namespace ICD.Connect.Cameras.Proxies.Devices
{
	public sealed class ProxyCameraWithZoom : AbstractProxyCameraDevice, ICameraWithZoom
	{
		/// <summary>
		/// Starts zooming the camera with the given action.
		/// </summary>
		/// <param name="action"></param>
		public void Zoom(eCameraZoomAction action)
		{
			CallMethod(CameraApi.METHOD_ZOOM, action);
		}
	}
}
