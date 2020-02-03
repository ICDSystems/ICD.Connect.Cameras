using System;

namespace ICD.Connect.Cameras.Controls
{
	[Flags]
	public enum eCameraFeatures
	{
		/// <summary>
		/// The control supports no features
		/// </summary>
		None = 0,

		/// <summary>
		/// The control supports panning
		/// </summary>
		Pan = 1,

		/// <summary>
		/// The control supports tilting
		/// </summary>
		Tilt = 2,

		/// <summary>
		/// The control supports zooming
		/// </summary>
		Zoom = 4,

		/// <summary>
		/// The control supports presets
		/// </summary>
		Presets = 8,

		/// <summary>
		/// The control supports camera muting
		/// </summary>
		Mute = 16,

		/// <summary>
		/// The control supports sending the camera to a home position
		/// </summary>
		Home = 32,

		/// <summary>
		/// The control supports paning and tilting
		/// </summary>
		PanTilt = Pan | Tilt,

		/// <summary>
		/// The control supports paning, tilting, and zooming
		/// </summary>
		PanTiltZoom = PanTilt | Zoom
	}
}