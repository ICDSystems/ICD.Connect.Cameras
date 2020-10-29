using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Protocol.Network.Ports.Tcp;
using ICD.Connect.Protocol.SerialBuffers;

namespace ICD.Connect.Cameras.Vaddio
{
	public sealed class VaddioRoboshotSerialBuffer : AbstractSerialBuffer
	{
		private const char DELIMITER = '>';

		/// <summary>
		/// Raised when a username prompt has been buffered.
		/// </summary>
		public event EventHandler OnUsernamePrompt;

		/// <summary>
		/// Raised when a password prompt has been buffered.
		/// </summary>
		public event EventHandler OnPasswordPrompt;

		/// <summary>
		/// Raised when a telnet header is discovered.
		/// </summary>
		public event EventHandler<StringEventArgs> OnSerialTelnetHeader;

		private string m_Remainder;

		/// <summary>
		/// Override to clear any current state.
		/// </summary>
		protected override void ClearFinal()
		{
			m_Remainder = string.Empty;
		}

		/// <summary>
		/// Override to process the given item for chunking.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected override IEnumerable<string> Process(string data)
		{
			// Prepend anything left from the previous pass
			m_Remainder = (m_Remainder ?? string.Empty) + data;
			if (m_Remainder.Length == 0)
				yield break;

			// First check for telnet negotiation
			while (m_Remainder.Length >= 3 && m_Remainder[0] == TelnetCommand.HEADER)
			{
				string output = m_Remainder.Substring(0, 3);
				m_Remainder = m_Remainder.Substring(3);
				OnSerialTelnetHeader.Raise(this, new StringEventArgs(output));
			}

			// Look for delimiters
			while (m_Remainder.Length > 0)
			{
				// Login prompt
				int index = m_Remainder.IndexOf("login:", StringComparison.Ordinal);
				if (index > 0 && !m_Remainder.Substring(0, index + "login:".Length).Contains("Last login:"))
				{
					m_Remainder = m_Remainder.Substring(index + "login:".Length);
					OnUsernamePrompt.Raise(this);
					continue;
				}

				// Password prompt
				index = m_Remainder.IndexOf("Password:", StringComparison.Ordinal);
				if (index > 0)
				{
					m_Remainder = m_Remainder.Substring(index + "Password:".Length);
					OnPasswordPrompt.Raise(this);
					continue;
				}

				// Regular > prompts
				index = m_Remainder.IndexOf(DELIMITER);
				if (index < 0)
					break;

				string output = m_Remainder.Substring(0, index);
				m_Remainder = m_Remainder.Substring(index + 1);

				if (!string.IsNullOrEmpty(output))
					yield return output;
			}
		}
	}
}
