using System;
using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Cameras.Controls
{
	public static class PresetControlConsole
	{
		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleNodeBase> GetConsoleNodes(IPresetControl instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield break;
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="addRow"></param>
		public static void BuildConsoleStatus(IPresetControl instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			addRow("Max Presets", instance.MaxPresets);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleCommand> GetConsoleCommands(IPresetControl instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield return
				new GenericConsoleCommand<int>("ActivatePreset", "Activates the preset with the given index.",
				                               preset => instance.ActivatePreset(preset));
			yield return
				new GenericConsoleCommand<int>("StorePreset", "Stores a preset with the given index.",
				                               preset => instance.StorePreset(preset));
		}
	}
}
