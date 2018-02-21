namespace ICD.Connect.Cameras
{
	public interface ICameraWithZoom : ICameraDevice
	{
		void Zoom(eCameraZoomAction action);
	}
}