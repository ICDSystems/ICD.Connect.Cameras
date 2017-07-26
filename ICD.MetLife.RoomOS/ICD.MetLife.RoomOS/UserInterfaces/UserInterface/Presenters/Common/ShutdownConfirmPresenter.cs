using System;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Common;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Common;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Common
{
	public sealed class ShutdownConfirmPresenter : AbstractPresenter<IShutdownConfirmView>, IShutdownConfirmPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public ShutdownConfirmPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IShutdownConfirmView view)
		{
			base.Refresh(view);

			ushort seconds = Room == null ? (ushort)0 : (ushort)Room.ShutdownTimer.RemainingSeconds;
			view.SetRemainingSeconds(seconds);
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

			room.ShutdownTimer.OnMillisecondsChanged += ShutdownTimerOnMillisecondsChanged;
			room.ShutdownTimer.OnIsRunningChanged += ShutdownTimerOnIsRunningChanged;
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

			room.ShutdownTimer.OnMillisecondsChanged -= ShutdownTimerOnMillisecondsChanged;
			room.ShutdownTimer.OnIsRunningChanged -= ShutdownTimerOnIsRunningChanged;
		}

		/// <summary>
		/// Called when the shutdown timer increments.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ShutdownTimerOnMillisecondsChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the room shutdown timer starts/stops.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ShutdownTimerOnIsRunningChanged(object sender, BoolEventArgs args)
		{
			ShowView(args.Data);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IShutdownConfirmView view)
		{
			base.Subscribe(view);

			view.OnCancelButtonPressed += ViewOnCancelButtonPressed;
			view.OnShutdownButtonPressed += ViewOnShutdownButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IShutdownConfirmView view)
		{
			base.Unsubscribe(view);

			view.OnCancelButtonPressed -= ViewOnCancelButtonPressed;
			view.OnShutdownButtonPressed -= ViewOnShutdownButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the shutdown button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnShutdownButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Room.Shutdown();
		}

		/// <summary>
		/// Called when the user presses the cancel button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCancelButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Room.ShutdownTimer.Stop();
		}

		#endregion
	}
}
