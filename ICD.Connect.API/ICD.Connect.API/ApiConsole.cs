﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
#if SIMPLSHARP
using Crestron.SimplSharp;
#endif

namespace ICD.Connect.API
{
	/// <summary>
	/// Singleton representing the root node of the console.
	/// </summary>
	public sealed class ApiConsole : IConsoleNode
	{
		private const string ROOT_COMMAND = "ICD";
		private const string ROOT_HELP = "Root node for dynamic console commands";

		public const string ALL_COMMAND = "A";
		public const string HELP_COMMAND = "?";
		public const string STATUS_COMMAND = "STATUS";

		private static readonly ApiConsole s_Singleton;
		private static readonly IcdHashSet<IConsoleNodeBase> s_Children;
		private static readonly SafeCriticalSection s_ChildrenSection;

		#region Properties

		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		public string ConsoleName { get { return ROOT_COMMAND; } }

		/// <summary>
		/// Gets the help information for the node.
		/// </summary>
		public string ConsoleHelp { get { return ROOT_HELP; } }

		#endregion

		/// <summary>
		/// Static constructor.
		/// </summary>
		static ApiConsole()
		{
			s_Singleton = new ApiConsole();
			s_Children = new IcdHashSet<IConsoleNodeBase>();
			s_ChildrenSection = new SafeCriticalSection();

			IcdConsole.AddNewConsoleCommand(ExecuteCommand, ROOT_COMMAND, ROOT_HELP, IcdConsole.eAccessLevel.Operator);
		}

		#region Methods

		/// <summary>
		/// Adds the console node to the root of the dynamic console.
		/// </summary>
		/// <param name="node"></param>
		[PublicAPI]
		public static void RegisterChild(IConsoleNodeBase node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			s_ChildrenSection.Execute(() => s_Children.Add(node));
		}

		/// <summary>
		/// Removes the console node to the root of the dynamic console.
		/// </summary>
		/// <param name="node"></param>
		[PublicAPI]
		public static void UnregisterChild(IConsoleNodeBase node)
		{
			s_ChildrenSection.Execute(() => s_Children.Remove(node));
		}

		/// <summary>
		/// Called when the user runs the ICD command.
		/// </summary>
		/// <param name="command"></param>
		[PublicAPI]
		public static void ExecuteCommand(string command)
		{
			command = command.Trim();

			if (command.Equals("mother", StringComparison.OrdinalIgnoreCase))
			{
				IcdConsole.ConsoleCommandResponseLine("PRIORITY ONE");
				IcdConsole.ConsoleCommandResponseLine("INSURE RETURN OF ORGANISM");
				IcdConsole.ConsoleCommandResponseLine("FOR ANALYSIS.");
				IcdConsole.ConsoleCommandResponseLine("ALL OTHER CONSIDERATIONS SECONDARY.");
				IcdConsole.ConsoleCommandResponse("CREW EXPENDABLE.");
				return;
			}

			if (command.Equals("projection", StringComparison.OrdinalIgnoreCase))
			{
				IcdConsole.ConsoleCommandResponseLine("IF INTRUDER ORGANISM REACHES");
				IcdConsole.ConsoleCommandResponseLine("CIVILIZED AREAS . . .");
				IcdConsole.ConsoleCommandResponseLine("");
				IcdConsole.ConsoleCommandResponseLine("ENTIRE WORLD POPULATION INFECTED");
				IcdConsole.ConsoleCommandResponseLine("27,000 HOURS FROM FIRST CONTACT.");
				IcdConsole.ConsoleCommandResponseLine("");
				IcdConsole.ConsoleCommandResponseLine("PROBABILITY THAT ONE OR MORE");
				IcdConsole.ConsoleCommandResponseLine("TEAM MEMBERS MAY BE INFECTED");
				IcdConsole.ConsoleCommandResponseLine("BY INTRUDER ORGANISM:");
				IcdConsole.ConsoleCommandResponseLine("");
				IcdConsole.ConsoleCommandResponse("-75%-");
				return;
			}

			IcdConsole.ConsoleCommandResponseLine(s_Singleton.ExecuteConsoleCommand(command));
		}

#if SIMPLSHARP

		/// <summary>
		/// Called by S+ to execute a console command via UCMD.
		/// </summary>
		/// <param name="command"></param>
		[PublicAPI]
		public static void SPlusUcmd(SimplSharpString command)
		{
			string commandString = command == null ? String.Empty : command.ToString().Trim();
			commandString = String.IsNullOrEmpty(commandString) ? HELP_COMMAND : commandString;

			// User convenience, let them know there's actually a UCMD handler
			if (commandString == HELP_COMMAND)
			{
				IcdConsole.ConsoleCommandResponse("Type \"{0} {1}\" to see commands registered for {2}", ROOT_COMMAND, HELP_COMMAND,
						typeof(IcdConsole).Name);
				return;
			}

			// Only care about commands that start with ICD prefix.
			if (!commandString.Equals(ROOT_COMMAND, StringComparison.OrdinalIgnoreCase) &&
			    !commandString.StartsWith(ROOT_COMMAND + ' ', StringComparison.OrdinalIgnoreCase))
				return;

			// Trim the prefix
			commandString = commandString.Substring(ROOT_COMMAND.Length).Trim();

			ExecuteCommand(commandString);
		}
#endif

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			return s_ChildrenSection.Execute(() => s_Children.ToArray());
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			yield break;
		}

		#endregion
	}
}
