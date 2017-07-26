using System;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking.Keyboard;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking.Keyboard;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Blocking.Keyboard
{
	public sealed class PopupKeyboardSpecialPresenter : AbstractPresenter<IPopupKeyboardSpecialView>,
	                                                    IPopupKeyboardSpecialPresenter
	{
		public event PopupKeyboardKeyPressedCallback OnKeyPressed;

		private bool m_Shift;

		/// <summary>
		/// Gets/sets the shift state.
		/// </summary>
		public bool Shift
		{
			get { return m_Shift; }
			set
			{
				if (value == m_Shift)
					return;

				m_Shift = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public PopupKeyboardSpecialPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnKeyPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IPopupKeyboardSpecialView view)
		{
			base.Refresh(view);

			view.SetShift(m_Shift);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IPopupKeyboardSpecialView view)
		{
			base.Subscribe(view);

			view.OnAlphabetButtonPressed += ViewOnAlphabetButtonPressed;
			view.OnKeyPressed += ViewOnKeyPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IPopupKeyboardSpecialView view)
		{
			base.Unsubscribe(view);

			view.OnAlphabetButtonPressed -= ViewOnAlphabetButtonPressed;
			view.OnKeyPressed -= ViewOnKeyPressed;
		}

		/// <summary>
		/// Called when the user presses a key.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="key"></param>
		private void ViewOnKeyPressed(object sender, KeyboardKey key)
		{
			if (OnKeyPressed != null)
				OnKeyPressed(this, key);
		}

		/// <summary>
		/// Called when the user presses the alphabet button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnAlphabetButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
			Navigation.NavigateTo<IPopupKeyboardAlphaPresenter>();
		}

		#endregion
	}
}
