using System;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;
using ICD.Connect.Conferencing.ConferenceSources;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public sealed class KeyboardCommonPresenter : AbstractDialPresenter<IKeyboardCommonView>, IKeyboardCommonPresenter
	{
		private readonly KeypadStringBuilder m_StringBuilder;

		private IKeyboardAlphaPresenter m_AlphabetMenu;
		private IKeyboardSpecialPresenter m_SpecialMenu;

		private bool m_Shift;
		private bool m_Caps;
		private bool m_RefreshTextField;

		#region Properties

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Keyboard"; } }

		/// <summary>
		/// Gets the alphabet menu.
		/// </summary>
		private IKeyboardAlphaPresenter AlphabetMenu
		{
			get
			{
				if (m_AlphabetMenu != null)
					return m_AlphabetMenu;

				m_AlphabetMenu = Navigation.LazyLoadPresenter<IKeyboardAlphaPresenter>();
				Subscribe(m_AlphabetMenu);

				return m_AlphabetMenu;
			}
		}

		/// <summary>
		/// Gets the special menu.
		/// </summary>
		private IKeyboardSpecialPresenter SpecialMenu
		{
			get
			{
				if (m_SpecialMenu != null)
					return m_SpecialMenu;

				m_SpecialMenu = Navigation.LazyLoadPresenter<IKeyboardSpecialPresenter>();
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
		public KeyboardCommonPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
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
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IKeyboardCommonView view)
		{
			base.Refresh(view);

			view.SelectCapsButton(m_Caps);
			view.SelectShiftButton(m_Shift);

			string number = m_StringBuilder.ToString();
			eConferenceSourceType type = Room == null
				                             ? eConferenceSourceType.Unknown
				                             : Room.ConferenceManager.DialingPlan.GetSourceType(number);
			string dialType = StringUtils.NiceName(type);

			if (m_RefreshTextField)
				view.SetText(number);
			view.SetDialTypeText(dialType);

			bool enable = !string.IsNullOrEmpty(number);

			view.EnableDialButton(enable);
			view.EnableBackspaceButton(enable);
			view.EnableClearButton(enable);

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
		private void Subscribe(IKeyboardAlphaPresenter alphabetMenu)
		{
			alphabetMenu.OnKeyPressed += AlphabetMenuOnKeyPressed;
		}

		/// <summary>
		/// Unsubscribe from the alphabet keyboard events.
		/// </summary>
		/// <param name="alphabetMenu"></param>
		private void Unsubscribe(IKeyboardAlphaPresenter alphabetMenu)
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
		private void Subscribe(IKeyboardSpecialPresenter specialMenu)
		{
			specialMenu.OnKeyPressed += SpecialMenuOnKeyPressed;
		}

		/// <summary>
		/// Unsubscribe from the special keyboard menu.
		/// </summary>
		/// <param name="specialMenu"></param>
		private void Unsubscribe(IKeyboardSpecialPresenter specialMenu)
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
		protected override void Subscribe(IKeyboardCommonView view)
		{
			base.Subscribe(view);

			view.OnBackspaceButtonPressed += ViewOnBackspaceButtonPressed;
			view.OnCapsButtonPressed += ViewOnCapsButtonPressed;
			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnDialButtonPressed += ViewOnDialButtonPressed;
			view.OnShiftButtonPressed += ViewOnShiftButtonPressed;
			view.OnSpaceButtonPressed += ViewOnSpaceButtonPressed;
			view.OnTextEntered += ViewOnTextEntered;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IKeyboardCommonView view)
		{
			base.Unsubscribe(view);

			view.OnBackspaceButtonPressed -= ViewOnBackspaceButtonPressed;
			view.OnCapsButtonPressed -= ViewOnCapsButtonPressed;
			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnDialButtonPressed -= ViewOnDialButtonPressed;
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
		/// Called when the user presses the dial button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDialButtonPressed(object sender, EventArgs eventArgs)
		{
			Dial(m_StringBuilder.Pop());
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
			m_StringBuilder.Clear();

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
