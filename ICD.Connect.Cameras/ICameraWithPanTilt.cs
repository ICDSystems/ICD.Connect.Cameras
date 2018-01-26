namespace ICD.Connect.Cameras
{
	public interface ICameraWithPanTilt : ICameraDevice
	{
		void PanTilt(eCameraPanTiltAction action);
	}
}