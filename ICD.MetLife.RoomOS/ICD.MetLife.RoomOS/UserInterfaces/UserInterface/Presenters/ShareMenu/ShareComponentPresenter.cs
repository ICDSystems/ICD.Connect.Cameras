using System;
using System.Linq;
using ICD.Common.Services.Logging;
using ICD.Connect.Rooms.Extensions;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Settings.Core;
using ICD.Common.Properties;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.MetLife.RoomOS.Endpoints.Sources;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Share;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.ShareMenu;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.ShareMenu;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.ShareMenu
{
	public sealed class ShareComponentPresenter : AbstractComponentPresenter<IShareComponentView>,
	                                              IShareComponentPresenter
	{
		private const long HOLD_TIME = 1 * 1000;

		private const string STATUS_READY = "Ready";
		private const string STATUS_NOT_READY = "Not Ready";
		private const string STATUS_SHARING = "Sharing"; // TODO - Left/Right

		private MetlifeSource m_Source;

		private readonly SafeTimer m_HoldTimer;
		private bool m_Held;

		#region Properties

		/// <summary>
		/// Gets/sets the source.
		/// </summary>
		[CanBeNull]
		public MetlifeSource Source
		{
			get { return m_Source; }
			set
			{
				if (value == m_Source)
					return;

				m_Source = value;

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
		public ShareComponentPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_HoldTimer = SafeTimer.Stopped(HoldCallback);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_HoldTimer.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IShareComponentView view)
		{
			base.Refresh(view);

			bool enabled = m_Source != null && Room != null && Room.Routing.SourceDetected(m_Source, eConnectionType.Video);
			bool routed = m_Source != null && Room != null && Room.Routing.GetActiveDestinations(m_Source, eConnectionType.Video, true).Any();

			string name = m_Source == null ? null : m_Source.GetNameOrDeviceName(Room);
			string status = enabled ? STATUS_READY : STATUS_NOT_READY;
			if (routed)
				status = STATUS_SHARING;

			eSourceType sourceType = m_Source == null ? eSourceType.Laptop : m_Source.SourceType;
			eIconState iconState = routed ? eIconState.Active : eIconState.Default;

			IIcon icon = GetView().GetIcon(sourceType);

			view.Enable(enabled);
			view.SetLabel(name, status);
			view.SetIcon(icon, iconState);
		}

		#region Private Methods

		private void StopHoldTimer()
		{
			m_HoldTimer.Stop();
		}

		private void RestartHoldTimer()
		{
			m_HoldTimer.Reset(HOLD_TIME);
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

			room.Routing.OnSourceTransmissionStateChanged += RoomOnSourceTransmissionStateChanged;
			room.Routing.OnSourceDetectionStateChanged += RoomOnSourceDetectionStateChanged;
			room.Routing.OnRouteChanged += RoomOnRouteChanged;
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

			room.Routing.OnSourceTransmissionStateChanged -= RoomOnSourceTransmissionStateChanged;
			room.Routing.OnSourceDetectionStateChanged -= RoomOnSourceDetectionStateChanged;
			room.Routing.OnRouteChanged -= RoomOnRouteChanged;
		}

		/// <summary>
		/// Called when a source starts/stops sending video.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnSourceTransmissionStateChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when a source is connected or disconnected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnSourceDetectionStateChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the room
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnRouteChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IShareComponentView view)
		{
			base.Subscribe(view);

			view.OnPressed += ViewOnPressed;
			view.OnReleased += ViewOnReleased;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IShareComponentView view)
		{
			base.Unsubscribe(view);

			view.OnPressed -= ViewOnPressed;
			view.OnReleased -= ViewOnReleased;
		}

		/// <summary>
		/// Called when the user presses the component.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			m_Held = false;
			RestartHoldTimer();
		}

		/// <summary>
		/// Called when the user releases the component.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnReleased(object sender, EventArgs eventArgs)
		{
			if (m_Held)
				return;

			StopHoldTimer();

			if (Room == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to route source - room is null");
				return;
			}

			Room.Routing.Route(m_Source);

			// Share the source immediately if we are in a call
			if (Room.ConferenceManager.IsInCall)
				Room.StartPresentation();
		}

		/// <summary>
		/// Called when the user holds the component.
		/// </summary>
		private void HoldCallback()
		{
			m_Held = true;

			Navigation.NavigateTo<IDisplaySelectPresenter>().SetSource(m_Source);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			StopHoldTimer();
		}

		#endregion
	}
}
