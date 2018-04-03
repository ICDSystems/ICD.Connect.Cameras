using System;
using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Cameras.Controls
{
	public static class PanTiltControlConsole
	{
		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleNodeBase> GetConsoleNodes(IPanTiltControl instance)
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
		public static void BuildConsoleStatus(IPanTiltControl instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleCommand> GetConsoleCommands(IPanTiltControl instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield return new ConsoleCommand("Stop", "Sends the stop signal to the camera.", () => instance.Stop());
			yield return new ConsoleCommand("Up", "Sends the tilt up signal to the camera.", () => instance.TiltUp());
			yield return new ConsoleCommand("Down", "Sends the tilt down signal to the camera.", () => instance.TiltDown());
			yield return new ConsoleCommand("Left", "Sends the pan left signal to the camera.", () => instance.PanLeft());
			yield return new ConsoleCommand("Right", "Sends the pan right signal to the camera.", () => instance.PanRight());
		}
	}
}
