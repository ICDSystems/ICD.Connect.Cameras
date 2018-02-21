using ICD.Common.Properties;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public interface IZoomControl : IDeviceControl
	{
		void Stop();
		void ZoomIn();
		void ZoomOut();
	}
}