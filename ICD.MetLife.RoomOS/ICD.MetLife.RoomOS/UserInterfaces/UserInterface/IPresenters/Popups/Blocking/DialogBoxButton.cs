using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking
{
	public struct DialogBoxButton
	{
		private readonly string m_Name;
		private readonly Action m_Callback;

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name { get { return m_Name; } }

		/// <summary>
		/// Gets the callback.
		/// </summary>
		public Action Callback { get { return m_Callback; } }

		/// <summary>
		/// Constructor.
		/// Creates a button that doesn't do anything, i.e. a "cancel" button.
		/// </summary>
		/// <param name="name"></param>
		public DialogBoxButton(string name)
			: this(name, () => { })
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="callback"></param>
		public DialogBoxButton(string name, Action callback)
		{
			m_Name = name;
			m_Callback = callback;
		}
	}
}
