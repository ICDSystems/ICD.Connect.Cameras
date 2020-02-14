using System;
using System.Collections.Generic;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public abstract class AbstractCameraDeviceControl<TParent> : AbstractDeviceControl<TParent>, ICameraDeviceControl
		where TParent : IDeviceBase
	{
		public abstract event EventHandler<CameraControlPresetsChangedApiEventArgs> OnPresetsChanged;
		
		public abstract event EventHandler<CameraControlFeaturesChangedApiEventArgs> OnSupportedCameraFeaturesChanged;
		
		public abstract event EventHandler<CameraControlMuteChangedApiEventArgs> OnCameraMuteStateChanged;

		/// <summary>
		/// Gets the maximum number of presets this camera can support.
		/// </summary>
		public abstract int MaxPresets { get; }

		/// <summary>
		/// Gets whether the camera is currently muted
		/// </summary>
		public abstract bool IsCameraMuted { get; }

		public abstract eCameraFeatures SupportedCameraFeatures { get; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		protected AbstractCameraDeviceControl(TParent parent, int id)
			: base(parent, id)
		{
		}

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		public abstract void PanStop();

		/// <summary>
		/// Begin panning the camera to the left.
		/// </summary>
		public abstract void PanLeft();

		/// <summary>
		/// Begin panning the camera to the right.
		/// </summary>
		public abstract void PanRight();

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		public abstract void TiltStop();

		/// <summary>
		/// Begin tilting the camera up.
		/// </summary>
		public abstract void TiltUp();

		/// <summary>
		/// Begin tilting the camera down.
		/// </summary>
		public abstract void TiltDown();

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		public abstract void ZoomStop();

		/// <summary>
		/// Begin zooming the camera in.
		/// </summary>
		public abstract void ZoomIn();

		/// <summary>
		/// Begin zooming the camera out.
		/// </summary>
		public abstract void ZoomOut();

		/// <summary>
		/// Gets the stored camera presets.
		/// </summary>
		public abstract IEnumerable<CameraPreset> GetPresets();

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		public abstract void ActivatePreset(int presetId);

		/// <summary>
		/// Stores the cameras current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		public abstract void StorePreset(int presetId);

		/// <summary>
		/// Sets if the camera mute state should be active
		/// </summary>
		/// <param name="enable"></param>
		public abstract void MuteCamera(bool enable);

		/// <summary>
		/// Resets camera to its predefined home position
		/// </summary>
		public abstract void SendCameraHome();
	}
}
