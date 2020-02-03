namespace ICD.Connect.Cameras.Proxies.Controls
{
	public static class CameraControlApi
	{
		public const string EVENT_FEATURES_UPDATED = "OnFeaturesUpdated";
		public const string EVENT_PRESETS_UPDATED = "OnPresetsChanged";
		public const string EVENT_MUTE_CHANGED = "OnMuteChanged";

		public const string PROPERTY_MAX_PRESETS = "MaxPresets";
		public const string PROPERTY_SUPPORTED_FEATURES = "SupportedFeatures";
		public const string PROPERTY_MUTE_STATE = "MuteState";

		public const string METHOD_PAN_STOP = "PanStop";
		public const string METHOD_PAN_TILT = "PanTilt";
		public const string METHOD_PAN_LEFT = "PanLeft";
		public const string METHOD_PAN_RIGHT = "PanRight";
		public const string METHOD_TILT_STOP = "TiltStop";
		public const string METHOD_TILT_UP = "TiltUp";
		public const string METHOD_TILT_DOWN = "TiltDown";
		public const string METHOD_ZOOM_STOP = "ZoomStop";
		public const string METHOD_ZOOM = "Zoom";
		public const string METHOD_ZOOM_IN = "ZoomIn";
		public const string METHOD_ZOOM_OUT = "ZoomOut";
		public const string METHOD_GET_PRESETS = "GetPresets";
		public const string METHOD_ACTIVATE_PRESET = "ActivatePreset";
		public const string METHOD_STORE_PRESET = "StorePreset";
		public const string METHOD_SET_MUTE = "SetCameraMute";
		public const string METHOD_SEND_HOME = "SendHome";

		public const string HELP_EVENT_PRESETS_UPDATED = "Raised when the collection of presets is modified";
		public const string HELP_EVENT_FEATURES_UPDATED = "Raised when the supported features list is updated";
		public const string HELP_EVENT_MUTE_CHANGED = "Raised when the mute state changes on the camera.";

		public const string HELP_PROPERTY_MAX_PRESETS = "Gets the maximum number of presets this camera can support.";
		public const string HELP_PROPERTY_SUPPORTED_FEATURES = "Flags which indicate which features this camera can support.";
		public const string HELP_PROPERTY_MUTE_STATE = "Gets whether the camera is currently muted";

		public const string HELP_METHOD_STOP = "Stops the camera from moving.";
		public const string HELP_METHOD_PAN_LEFT = "Begin panning the camera to the left.";
		public const string HELP_METHOD_PAN_RIGHT = "Begin panning the camera to the right.";
		public const string HELP_METHOD_TILT_UP = "Begin tilting the camera up.";
		public const string HELP_METHOD_TILT_DOWN = "Begin tilting the camera down.";
		public const string HELP_METHOD_ZOOM_IN = "Begin zooming the camera in.";
		public const string HELP_METHOD_ZOOM_OUT = "Begin zooming the camera out.";
		public const string HELP_METHOD_GET_PRESETS = "Gets the stored camera presets.";
		public const string HELP_METHOD_ACTIVATE_PRESET = "Tells the camera to change its position to the given preset.";
		public const string HELP_METHOD_STORE_PRESET = "Stores the cameras current position in the given preset index.";
		public const string HELP_METHOD_SET_MUTE = "Sets if the camera mute state should be active.";
		public const string HELP_METHOD_SEND_HOME = "Resets camera to its predefined home position.";
	}
}
