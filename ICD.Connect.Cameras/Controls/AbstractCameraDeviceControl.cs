using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public abstract class AbstractCameraDeviceControl<TParent> : AbstractDeviceControl<TParent>, ICameraDeviceControl
		where TParent : IDeviceBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		protected AbstractCameraDeviceControl(TParent parent, int id)
			: base(parent, id)
		{
		}
	}
}
