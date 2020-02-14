using ICD.Connect.API.EventArguments;
using ICD.Connect.Cameras.Proxies.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class CameraControlFeaturesChangedApiEventArgs : AbstractGenericApiEventArgs<eCameraFeatures>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public CameraControlFeaturesChangedApiEventArgs(eCameraFeatures data) 
			: base(CameraControlApi.EVENT_FEATURES_UPDATED, data)
		{
		}
	}
}