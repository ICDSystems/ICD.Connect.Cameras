using System;
using System.Collections.Generic;
using System.Text;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Protocol.SerialBuffers;

namespace ICD.Connect.Cameras.Vaddio
{
	public sealed class VaddioRoboshotSerialBuffer : AbstractSerialBuffer
	{
		private static readonly char[] s_Delimiters =
		{
			'>',
			':' // Vaddio doesn't use CRLF after login prompts
		};

		/// <summary>
		/// Raised when a username prompt has been buffered.
		/// </summary>
		public event EventHandler OnUsernamePrompt;

		/// <summary>
		/// Raised when a password prompt has been buffered.
		/// </summary>
		public event EventHandler OnPasswordPrompt; 

		private readonly StringBuilder m_RxData;

		/// <summary>
		/// Constructor.
		/// </summary>
		public VaddioRoboshotSerialBuffer()
		{
			m_RxData = new StringBuilder();
		}

		/// <summary>
		/// Override to clear any current state.
		/// </summary>
		protected override void ClearFinal()
		{
			m_RxData.Clear();
		}

		/// <summary>
		/// Override to process the given item for chunking.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected override IEnumerable<string> Process(string data)
		{
			while (true)
			{
				int index = data.IndexOfAny(s_Delimiters);
				if (index < 0)
				{
					m_RxData.Append(data);
					break;
				}

				char delimiter = data[index];

				m_RxData.Append(data.Substring(0, index));
				data = data.Substring(index + 1);

				switch (delimiter)
				{
					// Login prompt
					case ':':
						string prompt = m_RxData.ToString().Trim().ToLower();

						if (prompt.EndsWith("login") && !prompt.EndsWith("last login"))
						{
							m_RxData.Clear();
							OnUsernamePrompt.Raise(this);
							continue;
						}

						if (prompt.EndsWith("password"))
						{
							m_RxData.Clear();
							OnPasswordPrompt.Raise(this);
							continue;
						}

						// Unhandled colon, push it back onto the rx data
						m_RxData.Append(':');
						break;
				}

				string output = m_RxData.Pop();
				if (!string.IsNullOrEmpty(output))
					yield return output;
			}
		}
	}
}
