using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.API.Attributes;
using ICD.Connect.Cameras.Proxies.Devices;

namespace ICD.Connect.Cameras.Devices
{
	[ApiClass(typeof(ProxyCameraWithPresets), typeof(ICameraDevice))]
	public interface ICameraWithPresets : ICameraDevice
	{
		/// <summary>
		/// Raised when the presets are changed.
		/// </summary>
		[PublicAPI]
		event EventHandler OnPresetsChanged;

		/// <summary>
		/// Gets the maximum number of presets this camera can support.
		/// </summary>
		[ApiProperty(CameraApi.PROPERTY_MAX_PRESETS, CameraApi.HELP_PROPERTY_MAX_PRESETS)]
		int MaxPresets { get; }

		/// <summary>
		/// Gets the stored camera presets.
		/// </summary>
		[ApiMethod(CameraApi.METHOD_GET_PRESETS, CameraApi.HELP_METHOD_GET_PRESETS)]
		IEnumerable<CameraPreset> GetPresets();

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		[ApiMethod(CameraApi.METHOD_ACTIVATE_PRESET, CameraApi.HELP_METHOD_ACTIVATE_PRESET)]
		void ActivatePreset(int presetId);

		/// <summary>
		/// Stores the cameras current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		[ApiMethod(CameraApi.METHOD_STORE_PRESET, CameraApi.HELP_METHOD_STORE_PRESET)]
		void StorePreset(int presetId);
	}
}