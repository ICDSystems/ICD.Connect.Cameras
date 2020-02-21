using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;

namespace ICD.Connect.Cameras.Devices
{
	public static class CameraDeviceConsole
	{
		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleNodeBase> GetConsoleNodes(ICameraDevice instance)
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
		public static void BuildConsoleStatus(ICameraDevice instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			addRow("Camera Mute", instance.IsCameraMuted ? "Enabled" : "Disabled");
			addRow("Max Presets", instance.MaxPresets);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleCommand> GetConsoleCommands(ICameraDevice instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			if (instance.SupportedCameraFeatures.HasFlag(eCameraFeatures.Home))
			{
				yield return new ConsoleCommand("ActivateHome", "Sends the camera to its home position", () => instance.ActivateHome());
				yield return new ConsoleCommand("StoreHome", "Stores the current position as the home position", () => instance.StoreHome());
			}

			if (instance.SupportedCameraFeatures.HasFlag(eCameraFeatures.Mute))
				yield return new GenericConsoleCommand<bool>("Mute", "Enables or Disables Camera Mute", a => instance.MuteCamera(a));

			string panHelp = string.Format("Pan <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eCameraPanAction>()));
			if (instance.SupportedCameraFeatures.HasFlag(eCameraFeatures.Pan))
				yield return new GenericConsoleCommand<eCameraPanAction>("Pan", panHelp, a => instance.Pan(a));

			if (instance.SupportedCameraFeatures.HasFlag(eCameraFeatures.Presets))
			{
				yield return new GenericConsoleCommand<int>("StorePreset", "StorePreset <ID>", p => instance.StorePreset(p));
				yield return new GenericConsoleCommand<int>("ActivatePreset", "ActivatePreset <ID>", p => instance.ActivatePreset(p));
				yield return new ConsoleCommand("PrintPresets", "Prints a table of the stored presets", () => PrintPresets(instance));
			}

			string tiltHelp = string.Format("Tilt <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eCameraTiltAction>()));
			if (instance.SupportedCameraFeatures.HasFlag(eCameraFeatures.Tilt))
				yield return new GenericConsoleCommand<eCameraTiltAction>("Tilt", tiltHelp, a => instance.Tilt(a));
	
			string zoomHelp = string.Format("Zoom <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eCameraZoomAction>()));
			if (instance.SupportedCameraFeatures.HasFlag(eCameraFeatures.Zoom))
				yield return new GenericConsoleCommand<eCameraZoomAction>("Zoom", zoomHelp, a => instance.Zoom(a));
		}

		private static string PrintPresets(ICameraDevice instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			TableBuilder builder = new TableBuilder("ID", "Name");

			foreach (CameraPreset preset in instance.GetPresets().OrderBy(p => p.PresetId))
				builder.AddRow(preset.PresetId, preset.Name);

			return builder.ToString();
		}
	}
}