using System;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class SettingsFusionPresenter : AbstractFusionPresenter<ISettingsFusionView>, ISettingsFusionPresenter
	{
		private MetlifeRoomSettings m_SubscribedRoomSettings;
		private ICoreSettings m_Settings;

		private MetlifeRoomSettings RoomSettings
		{
			get { return m_Settings.OriginatorSettings.GetById(Room.Id) as MetlifeRoomSettings; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
			: base(roomId, presenters, views, core)
		{
		}

		/// <summary>
		/// Refreshes the state of the view.
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();

			MetlifeRoomSettings settings = RoomSettings;

			string building = settings == null ? null : settings.Prefix;
			string roomName = settings == null ? null : settings.Name;
			string roomNumber = settings == null ? null : settings.Number;
			string roomOwner = settings == null ? null : settings.OwnerName;
			string roomPhoneNumber = settings == null ? null : settings.PhoneNumber;
			string roomType = roomName;

			GetView().SetBuilding(building);
			GetView().SetRoomName(roomName);
			GetView().SetRoomNumber(roomNumber);
			GetView().SetRoomOwner(roomOwner);
			GetView().SetRoomPhoneNumber(roomPhoneNumber);
			GetView().SetRoomType(roomType);
		}

		/// <summary>
		/// Applies the settings to the room.
		/// </summary>
		private void ApplySettings()
		{
			Core.ApplySettings(m_Settings);
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(MetlifeRoom room)
		{
			base.Subscribe(room);

			m_Settings = Core.CopySettings();

			m_SubscribedRoomSettings = RoomSettings;

			m_SubscribedRoomSettings.OnNameChanged += SettingsOnNameChanged;
			m_SubscribedRoomSettings.OnNumberChanged += SettingsOnNumberChanged;
			m_SubscribedRoomSettings.OnOwnerNameChanged += SettingsOnOwnerNameChanged;
			m_SubscribedRoomSettings.OnPhoneNumberChanged += SettingsOnPhoneNumberChanged;
			m_SubscribedRoomSettings.OnPrefixChanged += SettingsOnPrefixChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedRoomSettings == null)
				return;

			m_SubscribedRoomSettings.OnNameChanged -= SettingsOnNameChanged;
			m_SubscribedRoomSettings.OnNumberChanged -= SettingsOnNumberChanged;
			m_SubscribedRoomSettings.OnOwnerNameChanged -= SettingsOnOwnerNameChanged;
			m_SubscribedRoomSettings.OnPhoneNumberChanged -= SettingsOnPhoneNumberChanged;
			m_SubscribedRoomSettings.OnPrefixChanged -= SettingsOnPrefixChanged;
		}

		private void SettingsOnPrefixChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SettingsOnPhoneNumberChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SettingsOnOwnerNameChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SettingsOnNumberChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		private void SettingsOnNameChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshAsync();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsFusionView view)
		{
			base.Subscribe(view);

			view.OnApplyRoomSettings += ViewOnApplyRoomSettings;
			view.OnBuildingChanged += ViewOnBuildingChanged;
			view.OnRoomNameChanged += ViewOnRoomNameChanged;
			view.OnRoomNumberChanged += ViewOnRoomNumberChanged;
			view.OnRoomOwnerChanged += ViewOnRoomOwnerChanged;
			view.OnRoomPhoneNumberChanged += ViewOnRoomPhoneNumberChanged;
			view.OnRoomTypeChanged += ViewOnRoomTypeChanged;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsFusionView view)
		{
			base.Unsubscribe(view);

			view.OnApplyRoomSettings -= ViewOnApplyRoomSettings;
			view.OnBuildingChanged -= ViewOnBuildingChanged;
			view.OnRoomNameChanged -= ViewOnRoomNameChanged;
			view.OnRoomNumberChanged -= ViewOnRoomNumberChanged;
			view.OnRoomOwnerChanged -= ViewOnRoomOwnerChanged;
			view.OnRoomPhoneNumberChanged -= ViewOnRoomPhoneNumberChanged;
			view.OnRoomTypeChanged -= ViewOnRoomTypeChanged;
		}

		/// <summary>
		/// Called when the room type is changed via fusion.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnRoomTypeChanged(object sender, StringEventArgs args)
		{
			RoomSettings.Name = args.Data;
		}

		/// <summary>
		/// Called when the room phone number is changed via fusion.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnRoomPhoneNumberChanged(object sender, StringEventArgs args)
		{
			RoomSettings.PhoneNumber = args.Data;
		}

		/// <summary>
		/// Called when the room owner is changed via fusion.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnRoomOwnerChanged(object sender, StringEventArgs args)
		{
			RoomSettings.OwnerName = args.Data;
		}

		/// <summary>
		/// Called when the room number is changed via fusion.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnRoomNumberChanged(object sender, StringEventArgs args)
		{
			RoomSettings.Number = args.Data;
		}

		/// <summary>
		/// Called when the room name is changed via fusion.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnRoomNameChanged(object sender, StringEventArgs args)
		{
			RoomSettings.Name = args.Data;
		}

		/// <summary>
		/// Called when the room building is changed via fusion.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnBuildingChanged(object sender, StringEventArgs args)
		{
			RoomSettings.Prefix = args.Data;
		}

		/// <summary>
		/// Called when the room settings are applied via fusion.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnApplyRoomSettings(object sender, EventArgs args)
		{
			// Apply settings in a new thread to avoid disposing this panel in the UI thread.
			CrestronUtils.SafeInvoke(ApplySettings);
		}

		#endregion
	}
}
