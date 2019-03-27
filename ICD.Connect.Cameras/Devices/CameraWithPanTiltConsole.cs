using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Cameras.Devices
{
	public static class CameraWithPanTiltConsole
	{
		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleNodeBase> GetConsoleNodes(ICameraWithPanTilt instance)
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
		public static void BuildConsoleStatus(ICameraWithPanTilt instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleCommand> GetConsoleCommands(ICameraWithPanTilt instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			string panTiltHelp = string.Format("PanTilt <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eCameraPanTiltAction>()));

			yield return new GenericConsoleCommand<eCameraPanTiltAction>("PanTilt", panTiltHelp, a => instance.PanTilt(a));
		}
	}
}