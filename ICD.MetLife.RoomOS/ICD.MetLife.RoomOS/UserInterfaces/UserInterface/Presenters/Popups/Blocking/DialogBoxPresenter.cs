using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Blocking
{
	public sealed class DialogBoxPresenter : AbstractPresenter<IDialogBoxView>, IDialogBoxPresenter
	{
		private string m_Message;
		private readonly List<DialogBoxButton> m_Buttons;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public DialogBoxPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_Buttons = new List<DialogBoxButton>();
		}

		#region Methods

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IDialogBoxView view)
		{
			base.Refresh(view);

			view.SetMessage(m_Message);
			view.SetButtonLabels(m_Buttons.Select(b => b.Name));
		}

		/// <summary>
		/// Sets the dialog message and callbacks.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="buttons"></param>
		public void SetDialog(string message, IEnumerable<DialogBoxButton> buttons)
		{
			m_Message = message;

			m_Buttons.Clear();
			m_Buttons.AddRange(buttons);

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IDialogBoxView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IDialogBoxView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="uShortEventArgs"></param>
		private void ViewOnButtonPressed(object sender, UShortEventArgs uShortEventArgs)
		{
			// Clear the callback before calling it, just in case
			// the callback triggers a second confirmation popup.
			Action callback = m_Buttons[uShortEventArgs.Data].Callback;
			m_Buttons.Clear();

			if (callback != null)
				callback();

			ShowView(m_Buttons.Count > 0);
		}

		#endregion
	}
}
