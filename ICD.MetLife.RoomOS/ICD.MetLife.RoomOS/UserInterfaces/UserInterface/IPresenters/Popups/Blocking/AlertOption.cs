using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking
{
	/// <summary>
	/// An AlertOption is used to populate a button on the alert subpage.
	/// </summary>
	public struct AlertOption
	{
		private readonly string m_Name;
		private readonly Action m_Callback;

		/// <summary>
		/// The text that is shown on the button.
		/// </summary>
		public string Name { get { return m_Name; } }

		/// <summary>
		/// The delegate that is called when the button is pressed.
		/// </summary>
		public Action Callback { get { return m_Callback; } }

		/// <summary>
		/// Shorthand constructor for a simple close button.
		/// </summary>
		/// <param name="name"></param>
		public AlertOption(string name) : this(name, () => { })
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="callback"></param>
		public AlertOption(string name, Action callback)
		{
			m_Name = name;
			m_Callback = callback;
		}
	}
}
