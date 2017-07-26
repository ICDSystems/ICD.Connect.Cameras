namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews
{
	/// <summary>
	/// Simple pairing of uppercase and lowercase chars.
	/// </summary>
	public struct KeyboardKey
	{
		private readonly char m_Lower;
		private readonly char m_Upper;

		/// <summary>
		/// Returns true if the character is a letter.
		/// </summary>
		private bool IsLetter { get { return char.IsLetter(m_Lower); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="lower"></param>
		public KeyboardKey(char lower)
			: this(lower, char.ToUpper(lower))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="lower"></param>
		/// <param name="upper"></param>
		public KeyboardKey(char lower, char upper)
		{
			m_Lower = lower;
			m_Upper = upper;
		}

		/// <summary>
		/// Returns the character based on the shift/caps state.
		/// </summary>
		/// <param name="shift"></param>
		/// <param name="caps"></param>
		/// <returns></returns>
		public char GetChar(bool shift, bool caps)
		{
			bool upper = IsLetter ? shift ^ caps : shift;
			return upper ? m_Upper : m_Lower;
		}
	}
}
