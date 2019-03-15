using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Cameras.Devices
{
	public static class CameraWithZoomConsole
	{
		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleNodeBase> GetConsoleNodes(ICameraWithZoom instance)
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
		public static void BuildConsoleStatus(ICameraWithZoom instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleCommand> GetConsoleCommands(ICameraWithZoom instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			string zoomHelp = string.Format("Zoom <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eCameraZoomAction>()));

			yield return new GenericConsoleCommand<eCameraZoomAction>("Zoom", zoomHelp, a => instance.Zoom(a));
		}
	}
}
