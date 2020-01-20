using System;
using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Cameras.Controls
{
	public static class ZoomControlConsole
	{
		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleNodeBase> GetConsoleNodes(IZoomControl instance)
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
		public static void BuildConsoleStatus(IZoomControl instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleCommand> GetConsoleCommands(IZoomControl instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield return new ConsoleCommand("StopZ", "Sends the stop signal to the camera.", () => instance.Stop());
			yield return new ConsoleCommand("ZoomIn", "Sends the zoom in signal to the camera.", () => instance.ZoomIn());
			yield return new ConsoleCommand("ZoomOut", "Sends the zoom out signal to the camera.", () => instance.ZoomOut());
		}
	}
}
