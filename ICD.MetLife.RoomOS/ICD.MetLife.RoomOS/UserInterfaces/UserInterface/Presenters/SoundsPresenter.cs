using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters
{
	public sealed class SoundsPresenter : AbstractPresenter<ISoundsView>, ISoundsPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SoundsPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISoundsView view)
		{
			base.Refresh(view);

			IConference active = Room == null ? null : Room.ConferenceManager.ActiveConference;
			IConferenceSource[] sources = active == null ? new IConferenceSource[0] : active.GetSources().ToArray();
			bool ringtone = sources.Any(ShouldPlayRingtone);

			view.PlayRingtone(ringtone);
		}

		/// <summary>
		/// Returns true if the source should play a ringtone.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private bool ShouldPlayRingtone(IConferenceSource source)
		{
			if (source.Direction != eConferenceSourceDirection.Incoming)
				return false;

			switch (source.Status)
			{
				case eConferenceSourceStatus.Ringing:
				case eConferenceSourceStatus.Connecting:
					return true;
			}

			return false;
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

			room.ConferenceManager.OnRecentSourceAdded += RecentSourceAdded;
			room.ConferenceManager.OnActiveSourceStatusChanged += ActiveSourceStatusChanged;
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

			room.ConferenceManager.OnRecentSourceAdded -= RecentSourceAdded;
			room.ConferenceManager.OnActiveSourceStatusChanged -= ActiveSourceStatusChanged;
		}

		/// <summary>
		/// Called when a source status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ActiveSourceStatusChanged(object sender, ConferenceSourceStatusEventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when a new source is added to the system.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void RecentSourceAdded(object sender, ConferenceSourceEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
