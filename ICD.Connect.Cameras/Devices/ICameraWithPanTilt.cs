using ICD.Connect.API.Attributes;
using ICD.Connect.Cameras.Proxies.Devices;

namespace ICD.Connect.Cameras.Devices
{
	[ApiClass(typeof(ProxyCameraWithPanTilt), typeof(ICameraDevice))]
	public interface ICameraWithPanTilt : ICameraDevice
	{
		/// <summary>
		/// Starts rotating the camera with the given action.
		/// </summary>
		/// <param name="action"></param>
		[ApiMethod(CameraApi.METHOD_PAN_TILT, CameraApi.HELP_METHOD_PAN_TILT)]
		void PanTilt(eCameraPanTiltAction action);
	}
}
