using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking
{
	public interface IIncomingCallView : IView
	{
		/// <summary>
		/// Raised when the user presses the reject button.
		/// </summary>
		event EventHandler OnRejectButtonPressed;

		/// <summary>
		/// Raised when the user presses the answer button.
		/// </summary>
		event EventHandler OnAnswerButtonPressed;

		/// <summary>
		/// Sets the incoming/outgoing call text.
		/// </summary>
		/// <param name="message"></param>
		void SetMessageText(string message);

		/// <summary>
		/// Sets the visibility of the accept button.
		/// </summary>
		/// <param name="show"></param>
		void ShowAcceptButton(bool show);

		/// <summary>
		/// Sets the visibility of the cancel button.
		/// </summary>
		/// <param name="show"></param>
		void ShowCancelButton(bool show);

		/// <summary>
		/// Sets the visibility of the reject button.
		/// </summary>
		/// <param name="show"></param>
		void ShowRejectButton(bool show);
	}
}
