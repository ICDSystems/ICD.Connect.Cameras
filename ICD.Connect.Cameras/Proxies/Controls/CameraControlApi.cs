namespace ICD.Connect.Cameras.Proxies.Controls
{
	public static class CameraControlApi
	{
		public const string PROPERTY_MAX_PRESETS = "MaxPresets";

		public const string METHOD_STOP = "Stop";
		public const string METHOD_PAN_TILT = "PanTilt";
		public const string METHOD_PAN_LEFT = "PanLeft";
		public const string METHOD_PAN_RIGHT = "PanRight";
		public const string METHOD_TILT_UP = "TiltUp";
		public const string METHOD_TILT_DOWN = "TiltDown";
		public const string METHOD_ZOOM = "Zoom";
		public const string METHOD_ZOOM_IN = "ZoomIn";
		public const string METHOD_ZOOM_OUT = "ZoomOut";
		public const string METHOD_GET_PRESETS = "GetPresets";
		public const string METHOD_ACTIVATE_PRESET = "ActivatePreset";
		public const string METHOD_STORE_PRESET = "StorePreset";

		public const string HELP_PROPERTY_MAX_PRESETS = "Gets the maximum number of presets this camera can support.";

		public const string HELP_METHOD_STOP = "Stops the camera from moving.";
		public const string HELP_METHOD_PAN_TILT = "Starts rotating the camera with the given action.";
		public const string HELP_METHOD_PAN_LEFT = "Begin panning the camera to the left.";
		public const string HELP_METHOD_PAN_RIGHT = "Begin panning the camera to the right.";
		public const string HELP_METHOD_TILT_UP = "Begin tilting the camera up.";
		public const string HELP_METHOD_TILT_DOWN = "Begin tilting the camera down.";
		public const string HELP_METHOD_ZOOM = "Starts zooming the camera with the given action.";
		public const string HELP_METHOD_ZOOM_IN = "Begin zooming the camera in.";
		public const string HELP_METHOD_ZOOM_OUT = "Begin zooming the camera out.";
		public const string HELP_METHOD_GET_PRESETS = "Gets the stored camera presets.";
		public const string HELP_METHOD_ACTIVATE_PRESET = "Tells the camera to change its position to the given preset.";
		public const string HELP_METHOD_STORE_PRESET = "Stores the cameras current position in the given preset index.";
	}
}
