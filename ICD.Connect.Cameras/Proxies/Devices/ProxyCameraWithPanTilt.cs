using ICD.Connect.Cameras.Devices;

namespace ICD.Connect.Cameras.Proxies.Devices
{
	public sealed class ProxyCameraWithPanTilt : AbstractProxyCameraDevice, ICameraWithPanTilt
	{
		/// <summary>
		/// Starts rotating the camera with the given action.
		/// </summary>
		/// <param name="action"></param>
		public void PanTilt(eCameraPanTiltAction action)
		{
			CallMethod(CameraApi.METHOD_PAN_TILT, action);
		}
	}
}
