using System;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Components.Directory;
using ICD.Connect.Rooms;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class RoomFusionPresenter : AbstractFusionPresenter<IRoomFusionView>, IRoomFusionPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public RoomFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
			: base(roomId, presenters, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();

			bool roomOccupied = Room.IsOccupied;
			string dialPlan = Room.DialingPlan.ConfigPath;

			GetView().SetRoomOccupied(roomOccupied);
			GetView().SetVoiceConferencingDialPlan(dialPlan);
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

			room.OnOccupiedStateChanged += RoomOnOccupiedStateChanged;
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

			room.OnOccupiedStateChanged -= RoomOnOccupiedStateChanged;
		}

		/// <summary>
		/// Called when the user enters/leaves the room.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void RoomOnOccupiedStateChanged(object sender, BoolEventArgs args)
		{
			RefreshAsync();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IRoomFusionView view)
		{
			base.Subscribe(view);

			view.OnUpdateContacts += ViewOnUpdateContacts;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IRoomFusionView view)
		{
			base.Unsubscribe(view);

			view.OnUpdateContacts -= ViewOnUpdateContacts;
		}

		/// <summary>
		/// Called when the user updates contacts via fusion.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnUpdateContacts(object sender, EventArgs eventArgs)
		{
			CiscoCodec codec = Room.GetDevice<CiscoCodec>();
			if (codec != null)
				codec.Components.GetComponent<DirectoryComponent>().Clear();
		}

		#endregion
	}
}
