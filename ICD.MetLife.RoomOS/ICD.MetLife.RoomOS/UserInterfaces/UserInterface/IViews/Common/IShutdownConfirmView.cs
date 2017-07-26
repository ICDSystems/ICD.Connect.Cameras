using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Common
{
	public interface IShutdownConfirmView : IView
	{
		/// <summary>
		/// Called when the user presses the cancel button.
		/// </summary>
		event EventHandler OnCancelButtonPressed;

		/// <summary>
		/// Called when the user presses the shutdown button.
		/// </summary>
		event EventHandler OnShutdownButtonPressed;

		/// <summary>
		/// Sets the number of remaining seconds until shutdown.
		/// </summary>
		/// <param name="seconds"></param>
		void SetRemainingSeconds(ushort seconds);
	}
}
