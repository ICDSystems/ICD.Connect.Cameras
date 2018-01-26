using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Cameras.Controls
{
	public class PresetControl<T> : AbstractDeviceControl<T>, IPresetControl
		where T : ICameraWithPresets
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public PresetControl(T parent, int id) : base(parent, id)
		{
		} 

		public event EventHandler OnPresetsChanged;

		/// <summary>
		/// Exposes the maximum number of presets this camera can support.
		/// </summary>
		public int MaxPresets { get { return Parent.MaxPresets; } }

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		public void ActivatePreset(int presetId)
		{
			Parent.ActivatePreset(presetId);
		}

		/// <summary>
		/// Stores the camera's current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		public void StorePreset(int presetId)
		{
			Parent.StorePreset(presetId);
			OnPresetsChanged.Raise(this);
		}

		/// <summary>
		/// Returns the presets, in order.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<CameraPreset> GetPresets()
		{
			return Parent.Presets.Select(p => p.Value).OrderBy(p => p.ListPosition).ToArray();
		}
	}
}