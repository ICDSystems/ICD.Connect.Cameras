using ICD.Connect.API.EventArguments;
using ICD.Connect.Cameras.Proxies.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class CameraControlMuteChangedApiEventArgs : AbstractGenericApiEventArgs<bool>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public CameraControlMuteChangedApiEventArgs(bool data)
			: base(CameraControlApi.EVENT_FEATURES_UPDATED, data)
		{
		}
	}
}