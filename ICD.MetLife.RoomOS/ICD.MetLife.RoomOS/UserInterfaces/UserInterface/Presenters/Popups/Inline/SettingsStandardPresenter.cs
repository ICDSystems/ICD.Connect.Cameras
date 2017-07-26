using System;
using System.Linq;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline
{
	public sealed class SettingsStandardPresenter : AbstractPopupPresenter<ISettingsStandardView>,
	                                                ISettingsStandardPresenter
	{
		private readonly KeypadStringBuilder m_StringBuilder;
		private IDevice[] m_Devices;

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Settings"; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsStandardPresenter(int roomId, INavigationController nav, IViewFactory views, ICore core)
			: base(roomId, nav, views, core)
		{
			m_StringBuilder = new KeypadStringBuilder();
			m_StringBuilder.OnStringChanged += StringBuilderOnStringChanged;

			m_Devices = new IDevice[0];
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsStandardView view)
		{
			base.Refresh(view);

			// Password
			view.SetPasswordText(m_StringBuilder.ToString());

			// Device status
			view.SetDeviceCount((ushort)m_Devices.Length);

			for (ushort index = 0; index < m_Devices.Length; index++)
			{
				IDevice device = m_Devices[index];
				eColor color = device.IsOnline ? eColor.Blue : eColor.Red;

				view.SetDeviceLabel(index, device.Name, color);
			}
		}

		/// <summary>
		/// Called when the string builder contents change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void StringBuilderOnStringChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(MetlifeRoom room)
		{
			base.Subscribe(room);

			m_Devices =
				room == null
					? new IDevice[0]
					: room.Devices
					      .OrderBy(d => d.Name)
					      .ToArray();

			foreach (IDevice device in m_Devices)
				device.OnIsOnlineStateChanged += DeviceOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			foreach (IDevice device in m_Devices)
				device.OnIsOnlineStateChanged += DeviceOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Called when a device online state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void DeviceOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshAsync();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsStandardView view)
		{
			base.Subscribe(view);

			view.OnClearButtonPressed += ViewOnClearButtonPressed;
			view.OnEnterButtonPressed += ViewOnEnterButtonPressed;
			view.OnKeypadButtonPressed += ViewOnKeypadButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsStandardView view)
		{
			base.Unsubscribe(view);

			view.OnClearButtonPressed -= ViewOnClearButtonPressed;
			view.OnEnterButtonPressed -= ViewOnEnterButtonPressed;
			view.OnKeypadButtonPressed -= ViewOnKeypadButtonPressed;
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
		/// Called when the user presses the enter button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnEnterButtonPressed(object sender, EventArgs eventArgs)
		{
			string password = m_StringBuilder.ToString();
			m_StringBuilder.Clear();

			// TODO - Pull from xml
			if (password != "1988")
				return;

			ShowView(false);
			Navigation.NavigateTo<ISettingsBasePresenter>();
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
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_StringBuilder.Clear();
		}

		#endregion
	}
}
