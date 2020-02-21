using System;
using System.Collections.Generic;
using ICD.Connect.API.Attributes;
using ICD.Connect.Cameras.Proxies.Controls;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Cameras.Controls
{
	[ApiClass(typeof(ProxyCameraDeviceControl), typeof(IDeviceControl))]
	public interface ICameraDeviceControl : IDeviceControl
	{
		#region Events

		/// <summary>
		/// Raised when the collection of presets is modified
		/// </summary>
		[ApiEvent(CameraControlApi.EVENT_PRESETS_UPDATED, CameraControlApi.HELP_EVENT_PRESETS_UPDATED)]
		event EventHandler<CameraControlPresetsChangedApiEventArgs> OnPresetsChanged;
		
		/// <summary>
		/// Raised when the supported features list is updated
		/// </summary>
		[ApiEvent(CameraControlApi.EVENT_FEATURES_UPDATED, CameraControlApi.HELP_EVENT_FEATURES_UPDATED)]
        event EventHandler<CameraControlFeaturesChangedApiEventArgs> OnSupportedCameraFeaturesChanged;

		/// <summary>
		/// Raised when the mute state changes on the camera.
		/// </summary>
		[ApiEvent(CameraControlApi.EVENT_MUTE_CHANGED, CameraControlApi.HELP_EVENT_MUTE_CHANGED)]
		event EventHandler<CameraControlMuteChangedApiEventArgs> OnCameraMuteStateChanged; 

		#endregion

		#region Properties

		/// <summary>
		/// Flags which indicate which features this camera can support
		/// </summary>
		[ApiProperty(CameraControlApi.PROPERTY_SUPPORTED_FEATURES, CameraControlApi.HELP_PROPERTY_SUPPORTED_FEATURES)]
		eCameraFeatures SupportedCameraFeatures { get; }

		/// <summary>
		/// Gets the maximum number of presets this camera can support
		/// </summary>
		[ApiProperty(CameraControlApi.PROPERTY_MAX_PRESETS, CameraControlApi.HELP_PROPERTY_MAX_PRESETS)]
		int MaxPresets { get; }

		/// <summary>
		/// Gets whether the camera is currently muted
		/// </summary>
		[ApiProperty(CameraControlApi.PROPERTY_MUTE_STATE, CameraControlApi.HELP_PROPERTY_MUTE_STATE)]
		bool IsCameraMuted { get; }

		#endregion

		#region Methods

		#region Pan Tilt

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_PAN_STOP, CameraControlApi.HELP_METHOD_STOP)]
		void PanStop();

		/// <summary>
		/// Begin panning the camera to the left.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_PAN_LEFT, CameraControlApi.HELP_METHOD_PAN_LEFT)]
		void PanLeft();

		/// <summary>
		/// Begin panning the camera to the right.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_PAN_RIGHT, CameraControlApi.HELP_METHOD_PAN_RIGHT)]
		void PanRight();

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_TILT_STOP, CameraControlApi.HELP_METHOD_STOP)]
		void TiltStop();

		/// <summary>
		/// Begin tilting the camera up.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_TILT_UP, CameraControlApi.HELP_METHOD_TILT_UP)]
		void TiltUp();

		/// <summary>
		/// Begin tilting the camera down.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_TILT_DOWN, CameraControlApi.HELP_METHOD_TILT_DOWN)]
		void TiltDown();

		#endregion

		#region Zoom

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_ZOOM_STOP, CameraControlApi.HELP_METHOD_STOP)]
		void ZoomStop();

		/// <summary>
		/// Begin zooming the camera in.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_ZOOM_IN, CameraControlApi.HELP_METHOD_ZOOM_IN)]
		void ZoomIn();

		/// <summary>
		/// Begin zooming the camera out.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_ZOOM_OUT, CameraControlApi.HELP_METHOD_ZOOM_OUT)]
		void ZoomOut();

		#endregion

		#region Presets

		/// <summary>
		/// Gets the stored camera presets.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_GET_PRESETS, CameraControlApi.HELP_METHOD_GET_PRESETS)]
		IEnumerable<CameraPreset> GetPresets();

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		[ApiMethod(CameraControlApi.METHOD_ACTIVATE_PRESET, CameraControlApi.HELP_METHOD_ACTIVATE_PRESET)]
		void ActivatePreset(int presetId);

		/// <summary>
		/// Stores the cameras current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		[ApiMethod(CameraControlApi.METHOD_STORE_PRESET, CameraControlApi.HELP_METHOD_STORE_PRESET)]
		void StorePreset(int presetId);

		#endregion

		#region Camera Mute

		/// <summary>
		/// Sets if the camera mute state should be active
		/// </summary>
		/// <param name="enable"></param>
		[ApiMethod(CameraControlApi.METHOD_SET_MUTE, CameraControlApi.HELP_METHOD_SET_MUTE)]
		void MuteCamera(bool enable);

		#endregion

		#region Camera Home

		/// <summary>
		/// Resets camera to its predefined home position
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_ACTIVATE_HOME, CameraControlApi.HELP_METHOD_ACTIVATE_HOME)]
		void ActivateHome();

		/// <summary>
		/// Stores the current position as the home position.
		/// </summary>
		[ApiMethod(CameraControlApi.METHOD_STORE_HOME, CameraControlApi.HELP_METHOD_STORE_HOME)]
		void StoreHome();

		#endregion

		#endregion
	}
}
