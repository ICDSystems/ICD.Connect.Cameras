using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Cameras.Controls
{
	public static class CameraDeviceControlConsole
	{
		public static IEnumerable<IConsoleNodeBase> GetConsoleNodes(ICameraDeviceControl instance, eCameraFeatures features)
		{
			List<IConsoleNodeBase> nodes = new List<IConsoleNodeBase>();

			if (features.HasFlag(eCameraFeatures.Pan))
				 nodes.AddRange(GetPanConsoleNodes(instance));
			if(features.HasFlag(eCameraFeatures.Tilt))
				nodes.AddRange(GetTiltConsoleNodes(instance));
			if(features.HasFlag(eCameraFeatures.Zoom))
				nodes.AddRange(GetZoomConsoleNodes(instance));
			if(features.HasFlag(eCameraFeatures.Presets))
				nodes.AddRange(GetPresetConsoleNodes(instance));
			if(features.HasFlag(eCameraFeatures.Mute))
				nodes.AddRange(GetMuteConsoleNodes(instance));

			return nodes.Count == 0 ? Enumerable.Empty<IConsoleNodeBase>() : nodes;
		}

		public static void BuildConsoleStatus(ICameraDeviceControl instance, AddStatusRowDelegate addRow, eCameraFeatures features)
		{
			if (features.HasFlag(eCameraFeatures.Pan))
				BuildPanConsoleStatus(instance, addRow);
			if (features.HasFlag(eCameraFeatures.Tilt))
				BuildTiltConsoleStatus(instance, addRow);
			if (features.HasFlag(eCameraFeatures.Zoom))
				BuildZoomConsoleStatus(instance, addRow);
			if (features.HasFlag(eCameraFeatures.Presets))
				BuildPresetConsoleStatus(instance, addRow);
			if (features.HasFlag(eCameraFeatures.Mute))
				BuildMuteConsoleStatus(instance, addRow);
		}

		public static IEnumerable<IConsoleCommand> GetConsoleCommands(ICameraDeviceControl instance, eCameraFeatures features)
		{
			List<IConsoleCommand> nodes = new List<IConsoleCommand>();

			if (features.HasFlag(eCameraFeatures.Pan))
				nodes.AddRange(GetPanConsoleCommands(instance));
			if (features.HasFlag(eCameraFeatures.Tilt))
				nodes.AddRange(GetTiltConsoleCommands(instance));
			if (features.HasFlag(eCameraFeatures.Zoom))
				nodes.AddRange(GetZoomConsoleCommands(instance));
			if (features.HasFlag(eCameraFeatures.Presets))
				nodes.AddRange(GetPresetConsoleCommands(instance));
			if (features.HasFlag(eCameraFeatures.Mute))
				nodes.AddRange(GetMuteConsoleCommands(instance));

			return nodes.Count == 0 ? Enumerable.Empty<IConsoleCommand>() : nodes;
		}

		#region Pan

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private static IEnumerable<IConsoleNodeBase> GetPanConsoleNodes(ICameraDeviceControl instance)
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
		private static void BuildPanConsoleStatus(ICameraDeviceControl instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private static IEnumerable<IConsoleCommand> GetPanConsoleCommands(ICameraDeviceControl instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
		    
			yield return new ConsoleCommand("Left", "Sends the pan left signal to the camera.", () => instance.PanLeft());
			yield return new ConsoleCommand("Right", "Sends the pan right signal to the camera.", () => instance.PanRight());
			yield return new ConsoleCommand("PanStop", "Sends the stop signal to the camera.", () => instance.PanStop());
		}

		#endregion

		#region Tilt

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private static IEnumerable<IConsoleNodeBase> GetTiltConsoleNodes(ICameraDeviceControl instance)
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
		private static void BuildTiltConsoleStatus(ICameraDeviceControl instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private static IEnumerable<IConsoleCommand> GetTiltConsoleCommands(ICameraDeviceControl instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield return new ConsoleCommand("Up", "Sends the tilt up signal to the camera.", () => instance.TiltUp());
			yield return new ConsoleCommand("Down", "Sends the tilt down signal to the camera.", () => instance.TiltDown());
			yield return new ConsoleCommand("TiltStop", "Sends the stop signal to the camera.", () => instance.TiltStop());
		}

		#endregion

		#region Zoom

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private static IEnumerable<IConsoleNodeBase> GetZoomConsoleNodes(ICameraDeviceControl instance)
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
		private static void BuildZoomConsoleStatus(ICameraDeviceControl instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private static IEnumerable<IConsoleCommand> GetZoomConsoleCommands(ICameraDeviceControl instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield return new ConsoleCommand("ZoomIn", "Sends the zoom in signal to the camera.", () => instance.ZoomIn());
			yield return new ConsoleCommand("ZoomOut", "Sends the zoom out signal to the camera.", () => instance.ZoomOut());
			yield return new ConsoleCommand("ZoomStop", "Sends the stop signal to the camera.", () => instance.ZoomStop());
		}

		#endregion

		#region Presets

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private static IEnumerable<IConsoleNodeBase> GetPresetConsoleNodes(ICameraDeviceControl instance)
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
		private static void BuildPresetConsoleStatus(ICameraDeviceControl instance, AddStatusRowDelegate addRow)
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
		private static IEnumerable<IConsoleCommand> GetPresetConsoleCommands(ICameraDeviceControl instance)
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

		#endregion

		#region Mute

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		private static IEnumerable<IConsoleNodeBase> GetMuteConsoleNodes(ICameraDeviceControl instance)
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
		private static void BuildMuteConsoleStatus(ICameraDeviceControl instance, AddStatusRowDelegate addRow)
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
		private static IEnumerable<IConsoleCommand> GetMuteConsoleCommands(ICameraDeviceControl instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield break;
		}

		#endregion
	}
}