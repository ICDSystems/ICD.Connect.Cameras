using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Common;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Volume;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Common;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Common
{
	public sealed class StatusBarPresenter : AbstractPresenter<IStatusBarView>, IStatusBarPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public StatusBarPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			// Status bar starts visible
			Refresh();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IStatusBarView view)
		{
			base.Refresh(view);

			view.SetRoomPrefixText(Room == null ? null : Room.Prefix);
			view.SetRoomNameText(Room == null ? null : Room.Number);

			bool isInCall = Room != null && Room.ConferenceManager.IsInCall;
			bool isPrivacyMuted = Room != null && Room.ConferenceManager.PrivacyMuted;
			IConference conference = Room == null ? null : Room.ConferenceManager.ActiveConference;
			bool isOnHold = conference != null && conference.GetSources()
			                                                .Where(c => c.GetIsOnline())
			                                                .All(s => s.Status == eConferenceSourceStatus.OnHold);

			eColor color = eColor.Default;
			if (isInCall)
			{
				color = eColor.Green;
				if (isPrivacyMuted)
					color = eColor.Red;
				if (isOnHold)
					color = eColor.Yellow;
			}

			view.SetColor(color);
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(MetlifeRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.ConferenceManager.OnPrivacyMuteStatusChange += ConferenceManagerOnPrivacyMuteStatusChange;
			room.ConferenceManager.OnInCallChanged += ConferenceManagerOnInCallChanged;
			room.ConferenceManager.OnActiveSourceStatusChanged += ConferenceManagerOnActiveSourceStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.ConferenceManager.OnPrivacyMuteStatusChange -= ConferenceManagerOnPrivacyMuteStatusChange;
			room.ConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;
			room.ConferenceManager.OnActiveSourceStatusChanged -= ConferenceManagerOnActiveSourceStatusChanged;
		}

		/// <summary>
		/// Called when an active conference source status changes.
		/// We change the image color based on hold state.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConferenceManagerOnActiveSourceStatusChanged(object sender, ConferenceSourceStatusEventArgs e)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the we enter/leave a call.
		/// We change the image color based on in/out of call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ConferenceManagerOnInCallChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible(false);
		}

		/// <summary>
		/// Called when the privacy mute status changes.
		/// We change the image color based on mute status.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ConferenceManagerOnPrivacyMuteStatusChange(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IStatusBarView view)
		{
			base.Subscribe(view);

			view.OnAudioButtonPressed += ViewOnAudioButtonPressed;
			view.OnSettingsButtonPressed += ViewOnSettingsButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IStatusBarView view)
		{
			base.Unsubscribe(view);

			view.OnAudioButtonPressed -= ViewOnAudioButtonPressed;
			view.OnSettingsButtonPressed -= ViewOnSettingsButtonPressed;
		}

		/// <summary>
		/// Called when the settings button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSettingsButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.NavigateTo<ISettingsStandardPresenter>();
		}

		/// <summary>
		/// Called when the audio button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnAudioButtonPressed(object sender, EventArgs eventArgs)
		{
			IEnumerable<IVolumeDeviceControl> volumeControls = Room == null
				                                                   ? Enumerable.Empty<IVolumeDeviceControl>()
				                                                   : Room.GetVolumeControls();
			IVolumeDeviceControl[] controls = volumeControls as IVolumeDeviceControl[] ?? volumeControls.ToArray();

			if (controls.Length == 1)
			{
				IVolumeSidePresenter presenter = Navigation.LazyLoadPresenter<IVolumeSidePresenter>();
				presenter.VolumeControl = controls[0];
				presenter.ShowView(true);
			}
			else
			{
				Navigation.LazyLoadPresenter<IVolumePresenter>().ShowView(true);
			}
		}

		#endregion
	}
}
