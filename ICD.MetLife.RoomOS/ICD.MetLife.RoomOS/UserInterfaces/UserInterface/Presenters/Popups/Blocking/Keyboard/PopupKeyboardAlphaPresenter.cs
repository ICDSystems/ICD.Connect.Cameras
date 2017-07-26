using System;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking.Keyboard;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking.Keyboard;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Blocking.Keyboard
{
	public sealed class PopupKeyboardAlphaPresenter : AbstractPresenter<IPopupKeyboardAlphaView>,
	                                                  IPopupKeyboardAlphaPresenter
	{
		public event PopupKeyboardKeyPressedCallback OnKeyPressed;

		private bool m_Caps;
		private bool m_Shift;

		#region Properties

		/// <summary>
		/// Gets/sets the caps state.
		/// </summary>
		public bool Caps
		{
			get { return m_Caps; }
			set
			{
				if (value == m_Caps)
					return;

				m_Caps = value;

				RefreshIfVisible();
			}
		}

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

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public PopupKeyboardAlphaPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
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
		protected override void Refresh(IPopupKeyboardAlphaView view)
		{
			base.Refresh(view);

			view.SetShift(m_Shift, m_Caps);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IPopupKeyboardAlphaView view)
		{
			base.Subscribe(view);

			view.OnKeyPressed += ViewOnKeyPressed;
			view.OnSpecialButtonPressed += ViewOnSpecialButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IPopupKeyboardAlphaView view)
		{
			base.Unsubscribe(view);

			view.OnKeyPressed -= ViewOnKeyPressed;
			view.OnSpecialButtonPressed -= ViewOnSpecialButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the special button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSpecialButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
			Navigation.NavigateTo<IPopupKeyboardSpecialPresenter>();
		}

		/// <summary>
		/// Called when the user presses a key button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="key"></param>
		private void ViewOnKeyPressed(object sender, KeyboardKey key)
		{
			if (OnKeyPressed != null)
				OnKeyPressed(this, key);
		}

		#endregion
	}
}
