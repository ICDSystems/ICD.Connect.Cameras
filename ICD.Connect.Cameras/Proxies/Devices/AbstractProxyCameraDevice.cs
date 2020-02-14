using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Devices.Proxies.Devices;

namespace ICD.Connect.Cameras.Proxies.Devices
{
	public abstract class AbstractProxyCameraDevice<TSettings> : AbstractProxyDevice<TSettings>, IProxyCameraDevice
		where TSettings : IProxyCameraDeviceSettings
	{
		public event EventHandler<GenericEventArgs<IEnumerable<CameraPreset>>> OnPresetsChanged;
		public event EventHandler<GenericEventArgs<eCameraFeatures>> OnSupportedCameraFeaturesChanged;
		public event EventHandler<BoolEventArgs> OnCameraMuteStateChanged;

		/// <summary>
		/// Flags which indicate which features this camera can support
		/// </summary>
		public eCameraFeatures SupportedCameraFeatures { get; protected set; }

		/// <summary>
		/// Gets the maximum number of presets this camera can support
		/// </summary>
		public int MaxPresets { get; protected set; }

		/// <summary>
		/// Gets whether the camera is currently muted
		/// </summary>
		public bool IsCameraMuted { get; protected set; }

		/// <summary>
		/// Begins panning the camera
		/// </summary>
		/// <param name="action"></param>
		public void Pan(eCameraPanAction action)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Begin tilting the camera.
		/// </summary>
		public void Tilt(eCameraTiltAction action)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Zooms the camera.
		/// </summary>
		public void Zoom(eCameraZoomAction action)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the stored camera presets.
		/// </summary>
		public IEnumerable<CameraPreset> GetPresets()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		public void ActivatePreset(int presetId)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Stores the cameras current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		public void StorePreset(int presetId)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets if the camera mute state should be active
		/// </summary>
		/// <param name="enable"></param>
		public void MuteCamera(bool enable)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Resets camera to its predefined home position
		/// </summary>
		public void SendCameraHome()
		{
			throw new NotImplementedException();
		}
	}
}
