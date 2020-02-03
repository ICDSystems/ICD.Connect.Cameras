using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Cameras.Devices
{
	public static class CameraWithTiltConsole
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

			string tiltHelp = string.Format("PanTilt <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eCameraTiltAction>()));

			yield return new GenericConsoleCommand<eCameraTiltAction>("PanTilt", tiltHelp, a => instance.Tilt(a));
		}
	}
}