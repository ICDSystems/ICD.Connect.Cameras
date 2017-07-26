using System;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking.Keyboard;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings.SettingsDevicePropertiesComponents
{
	public abstract class AbstractSettingsDevicePropertiesKeyboardComponentPresenter :
		AbstractSettingsDevicePropertiesComponentPresenter
	{
		private IPopupKeyboardCommonPresenter m_PopupKeyboard;

		/// <summary>
		/// Gets the popup keyboard menu.
		/// </summary>
		private IPopupKeyboardCommonPresenter Keyboard
		{
			get { return m_PopupKeyboard ?? (m_PopupKeyboard = Navigation.LazyLoadPresenter<IPopupKeyboardCommonPresenter>()); }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractSettingsDevicePropertiesKeyboardComponentPresenter(int room, INavigationController nav,
		                                                                     IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Gets the value from the property.
		/// </summary>
		/// <returns></returns>
		protected abstract string GetStringValue();

		/// <summary>
		/// Converts and sets the value to the property.
		/// </summary>
		/// <param name="value"></param>
		protected abstract void SetStringValue(string value);

		/// <summary>
		/// Called when the user presses the property button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			PopupKeyboard();
		}

		/// <summary>
		/// Shows the popup keyboard.
		/// </summary>
		private void PopupKeyboard()
		{
			string value = GetStringValue();

			Keyboard.SetCallback(value, KeyboardOnSubmitted);
			Keyboard.ShowView(true);
		}

		/// <summary>
		/// Called when the user submits a value via the keyboard.
		/// </summary>
		/// <param name="value"></param>
		private void KeyboardOnSubmitted(string value)
		{
			SetStringValue(value);
			RefreshIfVisible();
		}
	}
}
