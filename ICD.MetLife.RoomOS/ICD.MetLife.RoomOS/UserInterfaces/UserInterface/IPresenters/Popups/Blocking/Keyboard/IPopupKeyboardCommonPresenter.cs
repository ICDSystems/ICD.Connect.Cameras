using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking.Keyboard
{
	public interface IPopupKeyboardCommonPresenter : IPresenter
	{
		/// <summary>
		/// Sets the current string value and a callback for submitting the modified value.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="callback"></param>
		void SetCallback(string value, Action<string> callback);
	}
}
