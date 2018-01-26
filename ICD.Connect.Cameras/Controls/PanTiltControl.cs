using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class PanTiltControl<T> : AbstractDeviceControl<T>, IPanTiltControl
		where T : ICameraWithPanTilt
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public PanTiltControl(T parent, int id) : base(parent, id)
		{
		}

		#region IPanTiltControl
		public void Stop()
		{
			Parent.PanTilt(eCameraPanTiltAction.Stop);
		}

		public void PanLeft()
		{
			Parent.PanTilt(eCameraPanTiltAction.Left);
		}

		public void PanRight()
		{
			Parent.PanTilt(eCameraPanTiltAction.Right);
		}

		public void TiltUp()
		{
			Parent.PanTilt(eCameraPanTiltAction.Up);
		}

		public void TiltDown()
		{
			Parent.PanTilt(eCameraPanTiltAction.Down);
		}
		#endregion
	}
}