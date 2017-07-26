using System;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.Connect.Conferencing.Conferences;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.MainNav.Components
{
	public sealed class MainNavHoldCallComponentPresenter : AbstractMainNavComponentPresenter,
	                                                        IMainNavHoldCallComponentPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public MainNavHoldCallComponentPresenter(int room, INavigationController nav, IViewFactory views,
		                                         ICore core)
			: base(room, nav, views, core)
		{
		}

		#region Protected Methods

		/// <summary>
		/// Gets the label text for the component.
		/// </summary>
		/// <returns></returns>
		protected override string GetLabel()
		{
			IConference conference = Room == null ? null : Room.ConferenceManager.ActiveConference;
			if (conference == null)
				return string.Empty;

			string resume = conference.Status == eConferenceStatus.OnHold ? "Resume" : "Hold";
			string call = conference.SourcesCount > 1 ? "Calls" : "Call";

			return string.Format("{0} {1}", resume, call);
		}

		/// <summary>
		/// Gets the current icon state for the component.
		/// </summary>
		/// <returns></returns>
		protected override eIconState GetIconState()
		{
			IConference conference = Room == null ? null : Room.ConferenceManager.ActiveConference;
			bool onHold = conference != null && conference.Status == eConferenceStatus.OnHold;
			return onHold ? eIconState.Active : eIconState.Default;
		}

		/// <summary>
		/// Gets the icon for the component.
		/// </summary>
		/// <returns></returns>
		protected override IIcon GetIcon()
		{
			return GetView().GetIcon(eMainNavIcon.HoldCall);
		}

		/// <summary>
		/// Called when the component is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			IConference conference = Room == null ? null : Room.ConferenceManager.ActiveConference;
			if (conference == null)
				return;

			if (conference.Status == eConferenceStatus.OnHold)
				conference.Resume();
			else
				conference.Hold();
		}

		#endregion

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

			room.ConferenceManager.OnActiveConferenceStatusChanged += ConferenceManagerOnActiveConferenceStatusChanged;
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

			room.ConferenceManager.OnActiveConferenceStatusChanged -= ConferenceManagerOnActiveConferenceStatusChanged;
		}

		/// <summary>
		/// Called when the active conference status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnActiveConferenceStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
