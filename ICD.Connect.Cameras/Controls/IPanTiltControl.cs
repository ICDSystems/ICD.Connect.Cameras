using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public interface IPanTiltControl : IDeviceControl
	{
		void Stop();
		void PanLeft();
		void PanRight();
		void TiltUp();
		void TiltDown();
	}
}