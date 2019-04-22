using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
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

			yield return new ConsoleCommand("PrintPresets", "Prints a table of the stored presets", () => PrintPresets(instance));
		}

		private static string PrintPresets(IPresetControl instance)
		{
			TableBuilder builder = new TableBuilder("ID", "Name");

			foreach (CameraPreset preset in instance.GetPresets().OrderBy(p => p.PresetId))
				builder.AddRow(preset.PresetId, preset.Name);

			return builder.ToString();
		}
	}
}
