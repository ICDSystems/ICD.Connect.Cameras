namespace ICD.Connect.Cameras.Controls
{
	public interface IPanTiltControl : ICameraDeviceControl
	{
		void Stop();
		void PanLeft();
		void PanRight();
		void TiltUp();
		void TiltDown();
	}
}