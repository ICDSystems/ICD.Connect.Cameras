using System.Collections.Generic;

namespace ICD.Connect.Cameras
{
	public interface ICameraWithPresets : ICameraDevice
	{
		/// <summary>
		/// Exposes the maximum number of presets this camera can support
		/// </summary>
		int MaxPresets { get; }

		/// <summary>
		/// Dictionary of presets, indexed by integer.
		/// </summary>
		Dictionary<int, CameraPreset> Presets { get; }

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
	}
}