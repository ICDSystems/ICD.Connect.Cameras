using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial.DialNav
{
	public interface IDialNavHomeButtonView : IView
	{
		/// <summary>
		/// Called when the user presses the home button.
		/// </summary>
		event EventHandler OnPressed;
	}
}
