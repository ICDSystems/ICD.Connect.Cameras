using System;
using ICD.Common.EventArguments;

namespace ICD.Connect.UI.Utils
{
	/// <summary>
	/// Convenience object for building strings one button-press at a time.
	/// </summary>
	public sealed class KeypadStringBuilder
	{
		public event EventHandler<StringEventArgs> OnStringChanged;

		private string m_Output;

		/// <summary>
		/// Constructor.
		/// </summary>
		public KeypadStringBuilder()
		{
			m_Output = string.Empty;
		}

		#region Methods

		/// <summary>
		/// Sets the current string in the builder.
		/// </summary>
		/// <param name="value"></param>
		public void SetString(string value)
		{
			value = value ?? string.Empty;
			if (value == m_Output)
				return;

			m_Output = value;

			RaiseOnStringChanged();
		}

		/// <summary>
		/// Appends the given character to the string.
		/// </summary>
		/// <param name="character"></param>
		public void AppendCharacter(char character)
		{
			SetString(m_Output + character);
		}

		/// <summary>
		/// Removes the last character in the string.
		/// </summary>
		public void Backspace()
		{
			if (!string.IsNullOrEmpty(m_Output))
				SetString(m_Output.Substring(0, m_Output.Length - 1));
		}

		/// <summary>
		/// Clears the string.
		/// </summary>
		public void Clear()
		{
			SetString(string.Empty);
		}

		/// <summary>
		/// Clears the builder and returns the contents.
		/// </summary>
		/// <returns></returns>
		public string Pop()
		{
			string output = ToString();
			Clear();
			return output;
		}

		/// <summary>
		/// Gets the resulting string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return m_Output;
		}

		#endregion

		/// <summary>
		/// Raises the OnStringChanged event.
		/// </summary>
		private void RaiseOnStringChanged()
		{
			EventHandler<StringEventArgs> handler = OnStringChanged;
			if (handler != null)
				handler(this, new StringEventArgs(m_Output));
		}
	}
}
