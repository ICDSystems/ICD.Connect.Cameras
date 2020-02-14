using System.Collections.Generic;
using ICD.Connect.API.EventArguments;
using ICD.Connect.Cameras.Proxies.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class CameraControlPresetsChangedApiEventArgs : AbstractGenericApiEventArgs<IEnumerable<CameraPreset>>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public CameraControlPresetsChangedApiEventArgs(IEnumerable<CameraPreset> data) 
			: base(CameraControlApi.EVENT_PRESETS_UPDATED, data)
		{
		}
	}
}