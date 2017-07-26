namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking
{
	/// <summary>
	/// Model for the alert subpage.
	/// </summary>
	public sealed class Alert
	{
		private readonly string m_Title;
		private readonly string m_Message;
		private readonly AlertOption[] m_Options;

		/// <summary>
		/// Gets the title text.
		/// </summary>
		public string Title { get { return m_Title; } }

		/// <summary>
		/// Gets the message text.
		/// </summary>
		public string Message { get { return m_Message; } }

		/// <summary>
		/// Gets the options.
		/// </summary>
		public AlertOption[] Options { get { return m_Options; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <param name="options"></param>
		public Alert(string title, string message, params AlertOption[] options)
		{
			m_Title = title;
			m_Message = message;
			m_Options = options;
		}
	}
}
