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
	public sealed class DialpadPresenter : AbstractDialPresenter<IDialpadView>, IDialpadPresenter
	{
		private readonly KeypadStringBuilder m_StringBuilder;
		private bool m_RefreshTextEntry;

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Dialpad"; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public DialpadPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_StringBuilder = new KeypadStringBuilder();
			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;

			m_RefreshTextEntry = true;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IDialpadView view)
		{
			base.Refresh(view);

			string number = m_StringBuilder.ToString();
			bool empty = string.IsNullOrEmpty(number);
			eConferenceSourceType callType = Room == null
				                                 ? eConferenceSourceType.Unknown
				                                 : Room.ConferenceManager.DialingPlan.GetSourceType(number);
			string callTypeString = StringUtils.NiceName(callType);

			if (m_RefreshTextEntry)
				view.SetTextEntryText(number);
			view.SetCallTypeLabel(callTypeString);

			view.EnableClearButton(!empty);
			view.EnableBackspaceButton(!empty);
			view.EnableDialButton(!empty);
		}

		/// <summary>
		/// Called when the stringbuilder text value changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void StringBuilderOnStringChanged(object sender, StringEventArgs stringEventArgs)
		{
			// Refresh synchronously to avoid setting label text while the user is typing.
			RefreshIfVisible(false);
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IDialpadView view)
		{
			base.Subscribe(view);

			view.OnBackspaceButtonPressed += ViewOnBackspaceButtonPressed;
			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnDialButtonPressed += ViewOnDialButtonPressed;
			view.OnKeypadButtonPressed += ViewOnKeypadButtonPressed;
			view.OnTextEntryModified += ViewOnTextEntryModified;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IDialpadView view)
		{
			base.Unsubscribe(view);

			view.OnBackspaceButtonPressed -= ViewOnBackspaceButtonPressed;
			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnDialButtonPressed -= ViewOnDialButtonPressed;
			view.OnKeypadButtonPressed -= ViewOnKeypadButtonPressed;
			view.OnTextEntryModified -= ViewOnTextEntryModified;
		}

		/// <summary>
		/// Called when the user enters text directly into the text entry field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void ViewOnTextEntryModified(object sender, StringEventArgs stringEventArgs)
		{
			m_RefreshTextEntry = false; // Stops interference with user typing
			m_StringBuilder.SetString(stringEventArgs.Data);
			m_RefreshTextEntry = true;
		}

		/// <summary>
		/// Called when the user presses a keypad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="charEventArgs"></param>
		private void ViewOnKeypadButtonPressed(object sender, CharEventArgs charEventArgs)
		{
			m_StringBuilder.AppendCharacter(charEventArgs.Data);
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
		/// Called when the user presses the backspace button.
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
		}

		#endregion
	}
}
