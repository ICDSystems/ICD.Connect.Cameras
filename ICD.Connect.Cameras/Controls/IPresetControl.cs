using System;
using System.Collections.Generic;
using ICD.Common.Properties;

namespace ICD.Connect.Cameras.Controls
{
	public interface IPresetControl : ICameraDeviceControl
	{
		/// <summary>
		/// Raised when the presets are changed.
		/// </summary>
		[PublicAPI]
		event EventHandler OnPresetsChanged;

		/// <summary>
		/// Exposes the maximum number of presets this camera can support
		/// </summary>
		int MaxPresets { get; }

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		void ActivatePreset(int presetId);

		/// <summary>
		/// Stores the camera's current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		void StorePreset(int presetId);

		/// <summary>
		/// Returns the presets, in order.
		/// </summary>
		/// <returns></returns>
		IEnumerable<CameraPreset>GetPresets();

	}
}