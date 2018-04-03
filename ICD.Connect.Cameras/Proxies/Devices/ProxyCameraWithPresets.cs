using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.Cameras.Devices;

namespace ICD.Connect.Cameras.Proxies.Devices
{
	public sealed class ProxyCameraWithPresets : AbstractProxyCameraDevice, ICameraWithPresets
	{
		/// <summary>
		/// Exposes the maximum number of presets this camera can support.
		/// </summary>
		public int MaxPresets { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Dictionary of presets, indexed by integer.
		/// </summary>
		public IEnumerable<CameraPreset> GetPresets()
		{
			// TODO
			yield break;
		}

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		public void ActivatePreset(int presetId)
		{
			CallMethod(CameraApi.METHOD_ACTIVATE_PRESET, presetId);
		}

		/// <summary>
		/// Stores the camera's current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		public void StorePreset(int presetId)
		{
			CallMethod(CameraApi.METHOD_STORE_PRESET, presetId);
		}
	}
}
