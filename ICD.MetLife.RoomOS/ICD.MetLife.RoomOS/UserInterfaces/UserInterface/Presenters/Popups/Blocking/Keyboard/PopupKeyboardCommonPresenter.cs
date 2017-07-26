using System;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking.Keyboard;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking.Keyboard;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Blocking.Keyboard
{
	public sealed class PopupKeyboardCommonPresenter : AbstractPresenter<IPopupKeyboardCommonView>,
	                                                   IPopupKeyboardCommonPresenter
	{
		private readonly KeypadStringBuilder m_StringBuilder;

		private IPopupKeyboardAlphaPresenter m_AlphabetMenu;
		private IPopupKeyboardSpecialPresenter m_SpecialMenu;

		private bool m_Shift;
		private bool m_Caps;
		private bool m_RefreshTextField;

		private string m_SubmitLabel;
		private string m_CancelLabel;
		private Action<string> m_Callback;

		#region Properties

		/// <summary>
		/// Gets the alphabet menu.
		/// </summary>
		public IPopupKeyboardAlphaPresenter AlphabetMenu
		{
			get
			{
				if (m_AlphabetMenu != null)
					return m_AlphabetMenu;

				m_AlphabetMenu = Navigation.LazyLoadPresenter<IPopupKeyboardAlphaPresenter>();
				Subscribe(m_AlphabetMenu);

				return m_AlphabetMenu;
			}
		}

		/// <summary>
		/// Gets the special menu.
		/// </summary>
		public IPopupKeyboardSpecialPresenter SpecialMenu
		{
			get
			{
				if (m_SpecialMenu != null)
					return m_SpecialMenu;

				m_SpecialMenu = Navigation.LazyLoadPresenter<IPopupKeyboardSpecialPresenter>();
				Subscribe(m_SpecialMenu);

				return m_SpecialMenu;
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public PopupKeyboardCommonPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_StringBuilder = new KeypadStringBuilder();
			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;

			m_RefreshTextField = true;
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			if (m_AlphabetMenu != null)
				Unsubscribe(m_AlphabetMenu);

			if (m_SpecialMenu != null)
				Unsubscribe(m_SpecialMenu);

			base.Dispose();
		}

		/// <summary>
		/// Sets the current string value and a callback for submitting the modified value.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="callback"></param>
		public void SetCallback(string value, Action<string> callback)
		{
			SetCallback("Submit", "Cancel", value, callback);
		}

		/// <summary>
		/// Sets the string builder for the keyboard to append to.
		/// </summary>
		/// <param name="submitLabel">The label for the submit button.</param>
		/// <param name="cancelLabel"></param>
		/// <param name="value"></param>
		/// <param name="callback"></param>
		public void SetCallback(string submitLabel, string cancelLabel, string value, Action<string> callback)
		{
			m_SubmitLabel = submitLabel;
			m_CancelLabel = cancelLabel;
			m_Callback = callback;
			m_StringBuilder.SetString(value);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IPopupKeyboardCommonView view)
		{
			base.Refresh(view);

			view.SelectCapsButton(m_Caps);
			view.SelectShiftButton(m_Shift);

			string text = m_StringBuilder.ToString();

			if (m_RefreshTextField)
				view.SetText(text);

			view.SetSubmitButtonLabel(m_SubmitLabel);
			view.SetCancelButtonLabel(m_CancelLabel);

			bool enable = !string.IsNullOrEmpty(text);

			view.EnableBackspaceButton(enable);
			view.EnableClearButton(enable);
			view.EnableSubmitButton(true);

			AlphabetMenu.Shift = m_Shift;
			AlphabetMenu.Caps = m_Caps;
			SpecialMenu.Shift = m_Shift;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called when the stringbuilder string changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void StringBuilderOnStringChanged(object sender, StringEventArgs stringEventArgs)
		{
			// Refresh synchronously to avoid interfering with user input.
			RefreshIfVisible(false);
		}

		#endregion

		#region Alphabet Keys Callbacks

		/// <summary>
		/// Subscribe to the alphabet keyboard events.
		/// </summary>
		/// <param name="alphabetMenu"></param>
		private void Subscribe(IPopupKeyboardAlphaPresenter alphabetMenu)
		{
			alphabetMenu.OnKeyPressed += AlphabetMenuOnKeyPressed;
		}

		/// <summary>
		/// Unsubscribe from the alphabet keyboard events.
		/// </summary>
		/// <param name="alphabetMenu"></param>
		private void Unsubscribe(IPopupKeyboardAlphaPresenter alphabetMenu)
		{
			alphabetMenu.OnKeyPressed -= AlphabetMenuOnKeyPressed;
		}

		/// <summary>
		/// Called when the user presses an alphabet key.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="key"></param>
		private void AlphabetMenuOnKeyPressed(object sender, KeyboardKey key)
		{
			char character = key.GetChar(m_Shift, m_Caps);

			m_Shift = false;
			m_StringBuilder.AppendCharacter(character);
		}

		#endregion

		#region Special Keys Callbacks

		/// <summary>
		/// Subscribe to the special keyboard menu.
		/// </summary>
		/// <param name="specialMenu"></param>
		private void Subscribe(IPopupKeyboardSpecialPresenter specialMenu)
		{
			specialMenu.OnKeyPressed += SpecialMenuOnKeyPressed;
		}

		/// <summary>
		/// Unsubscribe from the special keyboard menu.
		/// </summary>
		/// <param name="specialMenu"></param>
		private void Unsubscribe(IPopupKeyboardSpecialPresenter specialMenu)
		{
			specialMenu.OnKeyPressed -= SpecialMenuOnKeyPressed;
		}

		/// <summary>
		/// Called when the user 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="key"></param>
		private void SpecialMenuOnKeyPressed(object sender, KeyboardKey key)
		{
			char character = key.GetChar(m_Shift, m_Caps);

			m_Shift = false;
			m_StringBuilder.AppendCharacter(character);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IPopupKeyboardCommonView view)
		{
			base.Subscribe(view);

			view.OnBackspaceButtonPressed += ViewOnBackspaceButtonPressed;
			view.OnCapsButtonPressed += ViewOnCapsButtonPressed;
			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnSubmitButtonPressed += ViewOnSubmitButtonPressed;
			view.OnCancelButtonPressed += ViewOnCancelButtonPressed;
			view.OnShiftButtonPressed += ViewOnShiftButtonPressed;
			view.OnSpaceButtonPressed += ViewOnSpaceButtonPressed;
			view.OnTextEntered += ViewOnTextEntered;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IPopupKeyboardCommonView view)
		{
			base.Unsubscribe(view);

			view.OnBackspaceButtonPressed -= ViewOnBackspaceButtonPressed;
			view.OnCapsButtonPressed -= ViewOnCapsButtonPressed;
			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnSubmitButtonPressed -= ViewOnSubmitButtonPressed;
			view.OnCancelButtonPressed -= ViewOnCancelButtonPressed;
			view.OnShiftButtonPressed -= ViewOnShiftButtonPressed;
			view.OnSpaceButtonPressed -= ViewOnSpaceButtonPressed;
			view.OnTextEntered -= ViewOnTextEntered;
		}

		/// <summary>
		/// Called when the user enters text directly in the text field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void ViewOnTextEntered(object sender, StringEventArgs stringEventArgs)
		{
			m_RefreshTextField = false;
			m_StringBuilder.SetString(stringEventArgs.Data);
			m_RefreshTextField = true;
		}

		/// <summary>
		/// Called when the user presses the space bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSpaceButtonPressed(object sender, EventArgs eventArgs)
		{
			m_StringBuilder.AppendCharacter(' ');
		}

		/// <summary>
		/// Called when the user presses the shift button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnShiftButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Shift = !m_Shift;
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the user presses the submit button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSubmitButtonPressed(object sender, EventArgs eventArgs)
		{
			// Clear the callback before calling it, just in case
			// the callback triggers a second keyboard popup.
			Action<string> callback = m_Callback;
			m_Callback = null;

			if (callback != null)
				callback(m_StringBuilder.Pop());

			ShowView(m_Callback != null);
		}

		/// <summary>
		/// Called when the user presses the cancel button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCancelButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Callback = null;
			ShowView(false);
		}

		/// <summary>
		/// Called when the user presses the clear button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnClearButtonPressed(object sender, EventArgs eventArgs)
		{
			m_StringBuilder.Clear();
		}

		/// <summary>
		/// Called when the user presses the caps button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCapsButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Caps = !m_Caps;
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the iser presses the backspace button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnBackspaceButtonPressed(object sender, EventArgs eventArgs)
		{
			m_StringBuilder.Backspace();
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			// Show one of the child presenters.
			if (args.Data)
			{
				AlphabetMenu.ShowView(true);
				return;
			}

			// Hide the child presenters
			if (m_AlphabetMenu != null)
				m_AlphabetMenu.ShowView(false);
			if (m_SpecialMenu != null)
				m_SpecialMenu.ShowView(false);

			m_Caps = false;
			m_Shift = false;
		}

		#endregion
	}
}
