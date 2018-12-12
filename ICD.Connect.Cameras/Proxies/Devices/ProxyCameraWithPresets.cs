using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.API;
using ICD.Connect.API.Info;
using ICD.Connect.Cameras.Devices;

namespace ICD.Connect.Cameras.Proxies.Devices
{
	public sealed class ProxyCameraWithPresets : AbstractProxyCameraDevice<ProxyCameraWithPresetsSettings>, ICameraWithPresets
	{
		/// <summary>
		/// Raised when the presets are changed.
		/// </summary>
		public event EventHandler OnPresetsChanged;

		/// <summary>
		/// Exposes the maximum number of presets this camera can support.
		/// </summary>
		public int MaxPresets { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			OnPresetsChanged = null;

			base.DisposeFinal(disposing);
		}

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

		/// <summary>
		/// Override to build initialization commands on top of the current class info.
		/// </summary>
		/// <param name="command"></param>
		protected override void Initialize(ApiClassInfo command)
		{
			base.Initialize(command);

			ApiCommandBuilder.UpdateCommand(command)
			                 .GetProperty(CameraApi.HELP_PROPERTY_MAX_PRESETS)
			                 .CallMethod(CameraApi.METHOD_GET_PRESETS)
			                 .CompleteMethod()
			                 .Complete();
		}
	}
}
