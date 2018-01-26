using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class ZoomControl<T> : AbstractDeviceControl<T>, IZoomControl
		where T : ICameraWithZoom
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public ZoomControl(T parent, int id)
			: base(parent, id)
		{
		}

		#region IZoomControl
		public void Stop()
		{
			Parent.Zoom(eCameraZoomAction.Stop);
		}

		public void ZoomIn()
		{
			Parent.Zoom(eCameraZoomAction.ZoomIn);
		}

		public void ZoomOut()
		{
			Parent.Zoom(eCameraZoomAction.ZoomOut);
		}
		#endregion
	}
}