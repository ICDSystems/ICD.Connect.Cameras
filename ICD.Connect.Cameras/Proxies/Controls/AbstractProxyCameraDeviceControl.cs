using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.API;
using ICD.Connect.API.Info;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Devices.Proxies.Controls;
using ICD.Connect.Devices.Proxies.Devices;

namespace ICD.Connect.Cameras.Proxies.Controls
{
	public abstract class AbstractProxyCameraDeviceControl : AbstractProxyDeviceControl, IProxyCameraDeviceControl
	{
		public event EventHandler<CameraControlFeaturesChangedApiEventArgs> OnSupportedCameraFeaturesChanged;
		public event EventHandler<CameraControlMuteChangedApiEventArgs> OnCameraMuteStateChanged;
		public event EventHandler<CameraControlPresetsChangedApiEventArgs> OnPresetsChanged;

		public int MaxPresets { get; private set; }

		public bool IsCameraMuted { get; private set; }

		public eCameraFeatures SupportedCameraFeatures { get; private set; }

		private List<CameraPreset> m_CachedPresets; 

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		protected AbstractProxyCameraDeviceControl(IProxyDevice parent, int id)
			: base(parent, id)
		{
			m_CachedPresets = new List<CameraPreset>();
		}

		/// <summary>
		/// Override to build initialization commands on top of the current class info.
		/// </summary>
		/// <param name="command"></param>
		protected override void Initialize(ApiClassInfo command)
		{
			base.Initialize(command);

			ApiCommandBuilder.UpdateCommand(command)
			                 .SubscribeEvent(CameraControlApi.EVENT_FEATURES_UPDATED)
			                 .SubscribeEvent(CameraControlApi.EVENT_PRESETS_UPDATED)
			                 .GetProperty(CameraControlApi.PROPERTY_MAX_PRESETS)
			                 .GetProperty(CameraControlApi.PROPERTY_SUPPORTED_FEATURES)
			                 .Complete();
		}

		/// <summary>
		/// Updates the proxy with event feedback info.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="result"></param>
		protected override void ParseEvent(string name, ApiResult result)
		{
			base.ParseEvent(name, result);

			switch (name)
			{
				case CameraControlApi.EVENT_PRESETS_UPDATED:
					m_CachedPresets = result.GetValue<IEnumerable<CameraPreset>>().ToList();
					break;

				case CameraControlApi.EVENT_FEATURES_UPDATED:
					SupportedCameraFeatures = result.GetValue<eCameraFeatures>();
					break;

				case CameraControlApi.EVENT_MUTE_CHANGED:
					IsCameraMuted = result.GetValue<bool>();
					break;
			}
		}

		/// <summary>
		/// Updates the proxy with a property result.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="result"></param>
		protected override void ParseProperty(string name, ApiResult result)
		{
			base.ParseProperty(name, result);

			switch (name)
			{
				case CameraControlApi.PROPERTY_MAX_PRESETS:
					MaxPresets = result.GetValue<int>();
					break;
				case CameraControlApi.PROPERTY_MUTE_STATE:
					IsCameraMuted = result.GetValue<bool>();
					break;
				case CameraControlApi.PROPERTY_SUPPORTED_FEATURES:
					SupportedCameraFeatures = result.GetValue<eCameraFeatures>();
					break;
			}
		}

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		public void PanStop()
		{
			CallMethod(CameraControlApi.METHOD_PAN_STOP);
		}

		/// <summary>
		/// Begin panning the camera to the left.
		/// </summary>
		public void PanLeft()
		{
			CallMethod(CameraControlApi.METHOD_PAN_LEFT);
		}

		/// <summary>
		/// Begin panning the camera to the right.
		/// </summary>
		public void PanRight()
		{
			CallMethod(CameraControlApi.METHOD_PAN_RIGHT);
		}

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		public void TiltStop()
		{
			CallMethod(CameraControlApi.METHOD_TILT_STOP);
		}

		/// <summary>
		/// Begin tilting the camera up.
		/// </summary>
		public void TiltUp()
		{
			CallMethod(CameraControlApi.METHOD_TILT_UP);
		}

		/// <summary>
		/// Begin tilting the camera down.
		/// </summary>
		public void TiltDown()
		{
			CallMethod(CameraControlApi.METHOD_TILT_DOWN);
		}

		/// <summary>
		/// Stops the camera from moving.
		/// </summary>
		public void ZoomStop()
		{
			CallMethod(CameraControlApi.METHOD_ZOOM_STOP);
		}

		/// <summary>
		/// Begin zooming the camera in.
		/// </summary>
		public void ZoomIn()
		{
			CallMethod(CameraControlApi.METHOD_ZOOM_IN);
		}

		/// <summary>
		/// Begin zooming the camera out.
		/// </summary>
		public void ZoomOut()
		{
			CallMethod(CameraControlApi.METHOD_ZOOM_OUT);
		}

		/// <summary>
		/// Gets the stored camera presets.
		/// </summary>
		public IEnumerable<CameraPreset> GetPresets()
		{
			return m_CachedPresets;
		}

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		public void ActivatePreset(int presetId)
		{
			CallMethod(CameraControlApi.METHOD_ACTIVATE_PRESET, CameraControlApi.HELP_METHOD_ACTIVATE_PRESET);
		}

		/// <summary>
		/// Stores the cameras current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		public void StorePreset(int presetId)
		{
			CallMethod(CameraControlApi.METHOD_STORE_PRESET, CameraControlApi.HELP_METHOD_STORE_PRESET);
		}

		/// <summary>
		/// Sets if the camera mute state should be active
		/// </summary>
		/// <param name="enable"></param>
		public void MuteCamera(bool enable)
		{
			CallMethod(CameraControlApi.METHOD_SET_MUTE, CameraControlApi.HELP_METHOD_SET_MUTE);
		}

		/// <summary>
		/// Resets camera to its predefined home position
		/// </summary>
		public void ActivateHome()
		{
			CallMethod(CameraControlApi.METHOD_ACTIVATE_HOME, CameraControlApi.HELP_METHOD_ACTIVATE_HOME);
		}

		/// <summary>
		/// Stores the current position as the home position.
		/// </summary>
		public void StoreHome()
		{
			CallMethod(CameraControlApi.METHOD_STORE_HOME, CameraControlApi.HELP_METHOD_STORE_HOME);
		}
	}
}
