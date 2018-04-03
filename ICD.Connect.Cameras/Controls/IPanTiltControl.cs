using ICD.Connect.API.Attributes;
using ICD.Connect.Cameras.Proxies.Controls;

namespace ICD.Connect.Cameras.Controls
{
	[ApiClass(typeof(ProxyPanTiltControl), typeof(ICameraDeviceControl))]
	public interface IPanTiltControl : ICameraDeviceControl
	{
		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_STOP, CameraControlApi.HELP_METHOD_STOP)]
		void Stop();

		/// <summary>
		/// Begin panning the camera to the left.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_PAN_LEFT, CameraControlApi.HELP_METHOD_PAN_LEFT)]
		void PanLeft();

		/// <summary>
		/// Begin panning the camera to the right.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_PAN_RIGHT, CameraControlApi.HELP_METHOD_PAN_RIGHT)]
		void PanRight();

		/// <summary>
		/// Begin tilting the camera up.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_TILT_UP, CameraControlApi.HELP_METHOD_TILT_UP)]
		void TiltUp();

		/// <summary>
		/// Begin tilting the camera down.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_TILT_DOWN, CameraControlApi.HELP_METHOD_TILT_DOWN)]
		void TiltDown();

		/// <summary>
		/// Perfoms the given pan/tilt action.
		/// </summary>
		/// <param name="action"></param>
		[ApiMethod(CameraControlApi.METHOD_PAN_TILT, CameraControlApi.HELP_METHOD_PAN_TILT)]
		void PanTilt(eCameraPanTiltAction action);
	}
}
