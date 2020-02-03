using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;

namespace ICD.Connect.Cameras.Devices
{
	public static class CameraWithPanConsole
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

			string panHelp = string.Format("Pan <{0}>", StringUtils.ArrayFormat(EnumUtils.GetValues<eCameraPanAction>()));

			yield return new GenericConsoleCommand<eCameraPanAction>("Pan", panHelp, a => instance.Pan(a));
		}
	}
}