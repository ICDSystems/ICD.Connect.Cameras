namespace ICD.Connect.Cameras.Devices
{
	public interface ICameraWithPanTilt : ICameraDevice
	{
		void PanTilt(eCameraPanTiltAction action);
	}
}