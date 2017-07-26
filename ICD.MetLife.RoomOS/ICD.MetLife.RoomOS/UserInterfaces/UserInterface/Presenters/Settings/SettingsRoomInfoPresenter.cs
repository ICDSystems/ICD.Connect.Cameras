using System;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking.Keyboard;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsRoomInfoPresenter : AbstractPresenter<ISettingsRoomInfoView>, ISettingsRoomInfoPresenter
	{
		private readonly AbstractSettingsRoomInfoButton[] m_Buttons;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsRoomInfoPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			ICoreSettings settings = nav.LazyLoadPresenter<ISettingsBasePresenter>().SettingsInstance;

			m_Buttons = new AbstractSettingsRoomInfoButton[]
			{
				new SettingsRoomPrefixButton(room, settings),
				new SettingsRoomNumberButton(room, settings),
				new SettingsRoomPhoneNumberButton(room, settings),
				new SettingsRoomConfigNameButton(room, settings),
				new SettingsRoomOwnerNameButton(room, settings),
				new SettingsRoomOwnerPhoneButton(room, settings),
				new SettingsRoomOwnerEmailButton(room, settings),
				new SettingsRoomIdButton(room, settings)
			};
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsRoomInfoView view)
		{
			base.Refresh(view);

			view.SetButtonLabels(m_Buttons.Select(b => b.GetLabel()));
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsRoomInfoView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsRoomInfoView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnButtonPressed(object sender, UShortEventArgs args)
		{
			AbstractSettingsRoomInfoButton button = m_Buttons[args.Data];

			string value = button.GetValue();
			Action<string> callback = str => UpdateButton(button, str);

			IPopupKeyboardCommonPresenter keyboard = Navigation.NavigateTo<IPopupKeyboardCommonPresenter>();
			keyboard.SetCallback(value, callback);
		}

		/// <summary>
		/// Called when the user submits a value via the keyboard.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="value"></param>
		private void UpdateButton(AbstractSettingsRoomInfoButton button, string value)
		{
			button.SetValue(value);
			RefreshIfVisible();
		}

		#endregion
	}
}
