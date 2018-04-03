using ICD.Connect.API.Attributes;
using ICD.Connect.Cameras.Proxies.Controls;

namespace ICD.Connect.Cameras.Controls
{
	[ApiClass(typeof(ProxyZoomControl), typeof(ICameraDeviceControl))]
	public interface IZoomControl : ICameraDeviceControl
	{
		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_STOP, CameraControlApi.HELP_METHOD_STOP)]
		void Stop();

		/// <summary>
		/// Begin zooming the camera in.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_ZOOM_IN, CameraControlApi.HELP_METHOD_ZOOM_IN)]
		void ZoomIn();

		/// <summary>
		/// Begin zooming the camera out.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_ZOOM_OUT, CameraControlApi.HELP_METHOD_ZOOM_OUT)]
		void ZoomOut();

		/// <summary>
		/// Performs the given zoom action.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_ZOOM, CameraControlApi.HELP_METHOD_ZOOM)]
		void Zoom(eCameraZoomAction action);
	}
}
