using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;

namespace ICD.Connect.Cameras.Controls
{
	public sealed class PresetControl<T> : AbstractCameraDeviceControl<T>, IPresetControl
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

		#region IPresetControl
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
			return Parent.GetPresets();
		}
		#endregion

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<int>("Activate Preset", "Sends the stop signal to the camera.", (preset) => ActivatePreset(preset));
		}

		/// <summary>
		/// Workaround for the "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}