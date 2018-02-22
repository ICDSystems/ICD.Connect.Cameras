namespace ICD.Connect.Cameras.Controls
{
	public interface IZoomControl : ICameraDeviceControl
	{
		void Stop();
		void ZoomIn();
		void ZoomOut();
	}
}