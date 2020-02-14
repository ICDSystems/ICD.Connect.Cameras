using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.API.Attributes;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Proxies.Devices;
using ICD.Connect.Devices;

namespace ICD.Connect.Cameras.Devices
{
	[ApiClass(typeof(ProxyCameraDevice), typeof(IDevice))]
	public interface ICameraDevice : IDevice
	{
		#region Events

		/// <summary>
		/// Raised when the collection of presets is modified
		/// </summary>
		event EventHandler<GenericEventArgs<IEnumerable<CameraPreset>>> OnPresetsChanged;

		/// <summary>
		/// Raised when the supported features list is updated
		/// </summary>
		event EventHandler<GenericEventArgs<eCameraFeatures>> OnSupportedCameraFeaturesChanged;

		/// <summary>
		/// Raised when the mute state changes on the camera.
		/// </summary>
		event EventHandler<BoolEventArgs> OnCameraMuteStateChanged;

		#endregion

		#region Properties

		/// <summary>
		/// Flags which indicate which features this camera can support
		/// </summary>
		eCameraFeatures SupportedCameraFeatures { get; }

		/// <summary>
		/// Gets the maximum number of presets this camera can support
		/// </summary>
		int MaxPresets { get; }

		/// <summary>
		/// Gets whether the camera is currently muted
		/// </summary>
		bool IsCameraMuted { get; }

		#endregion

		#region Methods

		#region PTZ

		/// <summary>
		/// Begins panning the camera
		/// </summary>
		/// <param name="action"></param>
		void Pan(eCameraPanAction action);

		/// <summary>
		/// Begin tilting the camera.
		/// </summary>
		void Tilt(eCameraTiltAction action);

		/// <summary>
		/// Zooms the camera.
		/// </summary>
		void Zoom(eCameraZoomAction action);

		#endregion

		#region Presets

		/// <summary>
		/// Gets the stored camera presets.
		/// </summary>
		IEnumerable<CameraPreset> GetPresets();

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		void ActivatePreset(int presetId);

		/// <summary>
		/// Stores the cameras current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		void StorePreset(int presetId);

		#endregion

		#region Camera Mute

		/// <summary>
		/// Sets if the camera mute state should be active
		/// </summary>
		/// <param name="enable"></param>
		void MuteCamera(bool enable);

		#endregion

		#region Camera Home

		/// <summary>
		/// Resets camera to its predefined home position
		/// </summary>
		void SendCameraHome();

		#endregion

		#endregion
	}
}

