using ICD.Connect.API.Attributes;
using ICD.Connect.Cameras.Proxies.Devices;

namespace ICD.Connect.Cameras.Devices
{
	[ApiClass(typeof(ProxyCameraWithZoom), typeof(ICameraDevice))]
	public interface ICameraWithZoom : ICameraDevice
	{
		/// <summary>
		/// Starts zooming the camera with the given action.
		/// </summary>
		/// <param name="action"></param>
		[ApiMethod(CameraApi.METHOD_ZOOM, CameraApi.HELP_METHOD_ZOOM)]
		void Zoom(eCameraZoomAction action);
	}
}