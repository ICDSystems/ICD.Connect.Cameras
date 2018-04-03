using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.API.Attributes;
using ICD.Connect.Cameras.Proxies.Controls;

namespace ICD.Connect.Cameras.Controls
{
	[ApiClass(typeof(ProxyPresetControl), typeof(ICameraDeviceControl))]
	public interface IPresetControl : ICameraDeviceControl
	{
		/// <summary>
		/// Raised when the presets are changed.
		/// </summary>
		[PublicAPI]
		event EventHandler OnPresetsChanged;

		/// <summary>
		/// Gets the maximum number of presets this camera can support.
		/// </summary>
		[ApiProperty(CameraControlApi.PROPERTY_MAX_PRESETS, CameraControlApi.HELP_PROPERTY_MAX_PRESETS)]
		int MaxPresets { get; }

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
	}
}
