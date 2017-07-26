﻿#if !SIMPLSHARP
using System.Reflection;
#endif
using System.Globalization;
using ICD.Common.Utils;

namespace ICD.Connect.API.Commands
{
	public abstract class AbstractConsoleCommand : IConsoleCommand
	{
		protected const string DEFAULT_RESPONSE = "Command complete";

		private readonly string m_Name;
		private readonly string m_Help;
		private readonly bool m_Hidden;

		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public string ConsoleName { get { return m_Name; } }

		/// <summary>
		/// Gets the help for the command.
		/// </summary>
		public string Help { get { return m_Help; } }

		public bool Hidden { get { return m_Hidden; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="help"></param>
		protected AbstractConsoleCommand(string name, string help)
			: this(name, help, false)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="help"></param>
		/// <param name="hidden"></param>
		protected AbstractConsoleCommand(string name, string help, bool hidden)
		{
			m_Name = name;
			m_Help = help;
			m_Hidden = hidden;
		}

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="parameters"></param>
		public abstract string Execute(params string[] parameters);

		/// <summary>
		/// Returns true if the number of parameters match the target count.
		/// Prints a message to the console if false.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="targetCount"></param>
		/// <returns></returns>
		protected bool ValidateParamsCount(string[] parameters, int targetCount)
		{
			if (targetCount == parameters.Length)
				return true;

			return false;
		}

		/// <summary>
		/// Converts the console value to the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		protected static T Convert<T>(string value)
		{
			// Type.IsEnum will be in .NET Standard 2.0
#if SIMPLSHARP
			if (typeof(T).IsEnum)
#else
            if (typeof(T).GetTypeInfo().IsEnum)
#endif
				return EnumUtils.Parse<T>(value, true);

			return (T)System.Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
		}
	}
}
