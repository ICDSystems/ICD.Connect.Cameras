namespace ICD.Connect.Cameras.Devices
{
	public interface ICameraWithZoom : ICameraDevice
	{
		void Zoom(eCameraZoomAction action);
	}
}