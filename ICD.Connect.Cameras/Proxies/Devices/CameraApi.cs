namespace ICD.Connect.Cameras.Proxies.Devices
{
	public static class CameraApi
	{
		public const string PROPERTY_MAX_PRESETS = "MaxPresets";

		public const string METHOD_PAN = "Pan";
		public const string METHOD_TILT = "Pan";
		public const string METHOD_ZOOM = "Zoom";
		public const string METHOD_GET_PRESETS = "GetPresets";
		public const string METHOD_ACTIVATE_PRESET = "ActivatePreset";
		public const string METHOD_STORE_PRESET = "StorePreset";

		public const string HELP_PROPERTY_MAX_PRESETS = "Gets the maximum number of presets this camera can support.";

		public const string HELP_METHOD_PAN = "Starts paning the camera with the given action.";
		public const string HELP_METHOD_TILT = "Starts tilting the camera with the given action.";
		public const string HELP_METHOD_ZOOM = "Starts zooming the camera with the given action.";
		public const string HELP_METHOD_GET_PRESETS = "Gets the stored camera presets.";
		public const string HELP_METHOD_ACTIVATE_PRESET = "Tells the camera to change its position to the given preset.";
		public const string HELP_METHOD_STORE_PRESET = "Stores the cameras current position in the given preset index.";
	}
}
