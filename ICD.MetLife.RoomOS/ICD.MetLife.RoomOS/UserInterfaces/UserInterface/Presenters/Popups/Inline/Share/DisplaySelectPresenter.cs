using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Connect.Rooms.Extensions;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.MetLife.RoomOS.Endpoints.Destinations;
using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Share;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Share;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Share
{
	public sealed class DisplaySelectPresenter : AbstractPopupPresenter<IDisplaySelectView>, IDisplaySelectPresenter
	{
		private const ushort HIDE_TIME = 30 * 1000;

		private readonly SafeTimer m_VisibilityTimer;
		private MetlifeSource m_Source;
		private MetlifeDestination[] m_Destinations;
		private readonly SafeCriticalSection m_RefreshSection;

		#region Properties

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Share"; } }

		/// <summary>
		/// Gets the current source to be assigned to the displays.
		/// </summary>
		public MetlifeSource Source
		{
			get { return m_Source; }
			private set
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
		public DisplaySelectPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_VisibilityTimer = SafeTimer.Stopped(() => ShowView(false));
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_VisibilityTimer.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IDisplaySelectView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				m_Destinations = GetDisplayDestinations().Prepend(null).ToArray();

				string sourceName = m_Source == null ? string.Empty : m_Source.GetNameOrDeviceName(Room);
				bool listenToSourceEnabled = m_Source != null && EnumUtils.HasFlag(m_Source.ConnectionType, eConnectionType.Audio);
				bool showVideoCallEnabled = Room != null && Room.Routing.GetOverrides().Any();
				bool showVideoCallButtonVisible = Room != null && Room.ConferenceManager.IsInVideoCall;

				view.SetSourceName(sourceName);
				view.SetListenToSourceButtonEnabled(listenToSourceEnabled);
				view.SetShowVideoCallButtonEnabled(showVideoCallEnabled);
				view.SetShowVideoCallButtonVisible(showVideoCallButtonVisible);
				view.SetDestinationLabels(m_Destinations.Select(d => d == null ? "Share with Call" : d.GetNameOrDeviceName(Room)));

				RefreshDestinationSelection();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Refreshes the selection state of the destination buttons.
		/// </summary>
		private void RefreshDestinationSelection()
		{
			m_RefreshSection.Enter();

			try
			{
				IDisplaySelectView view = GetView();

				for (ushort index = 0; index < m_Destinations.Length; index++)
				{
					MetlifeDestination destination = m_Destinations[index];

					bool selected = GetDestinationSelected(destination);
					bool visible = GetDestinationVisible(destination);

					view.SetDestinationSelected(index, selected);
					view.SetDestinationVisible(index, visible);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Sets the source to be sent to selected displays.
		/// </summary>
		/// <param name="source"></param>
		public void SetSource(MetlifeSource source)
		{
			Source = source;
		}

		/// <summary>
		/// Stops the visibility timer.
		/// </summary>
		[PublicAPI]
		public void StopVisibilityTimer()
		{
			m_VisibilityTimer.Stop();
		}

		/// <summary>
		/// Resets the visibility timer.
		/// </summary>
		[PublicAPI]
		public void ResetVisibilityTimer()
		{
			m_VisibilityTimer.Reset(HIDE_TIME);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the MetlifeDestinations that point to display devices.
		/// Hack - Prepends with null for the "Share with Call" item.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<MetlifeDestination> GetDisplayDestinations()
		{
			return Room == null ? Enumerable.Empty<MetlifeDestination>() : Room.Routing.GetDisplayDestinations();
		}

		/// <summary>
		/// Returns true if the current source is routed to the given destination.
		/// </summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		private bool GetDestinationSelected(MetlifeDestination destination)
		{
			if (m_Source == null)
				return false;

			if (destination == null)
				return m_Source != null && Room != null && Room.Routing.IsRoutedToCodec(m_Source);

			return Room != null && Room.Routing.IsRoutedToDestination(m_Source, destination, eConnectionType.Video, true);
		}

		/// <summary>
		/// Returns true if the current source is routed to the given destination.
		/// </summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		private bool GetDestinationVisible(MetlifeDestination destination)
		{
			if (destination != null)
				return true;

			return m_Source != null && Room != null && Room.ConferenceManager.IsInVideoCall;
		}

		/// <summary>
		/// Routes the source to the room audio destination.
		/// </summary>
		private void RouteAudio()
		{
			if (m_Source == null)
				throw new InvalidOperationException("No source selected.");

			if (Room == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to route audio - room is null");
				return;
			}

			Room.Routing.RouteAudio(m_Source);
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

			room.Routing.OnRouteChanged -= RoomOnRouteChanged;
		}

		/// <summary>
		/// Called when the active device routing changes.
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
		protected override void Subscribe(IDisplaySelectView view)
		{
			base.Subscribe(view);

			view.OnDestinationButtonPressed += ViewOnDestinationButtonPressed;
			view.OnDestinationsMovingChanged += ViewOnDestinationsMovingChanged;
			view.OnListenToSourceButtonPressed += ViewOnListenToSourceButtonPressed;
			view.OnShowVideoCallButtonPressed += ViewOnShowVideoCallButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IDisplaySelectView view)
		{
			base.Unsubscribe(view);

			view.OnDestinationButtonPressed -= ViewOnDestinationButtonPressed;
			view.OnDestinationsMovingChanged -= ViewOnDestinationsMovingChanged;
			view.OnListenToSourceButtonPressed -= ViewOnListenToSourceButtonPressed;
			view.OnShowVideoCallButtonPressed -= ViewOnShowVideoCallButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the Show Video Call button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnShowVideoCallButtonPressed(object sender, EventArgs args)
		{
			if (Room != null)
				Room.Routing.ClearOverrides();
			ResetVisibilityTimer();
		}

		/// <summary>
		/// Called when the user presses the Listen To Source button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnListenToSourceButtonPressed(object sender, EventArgs args)
		{
			RouteAudio();
			ResetVisibilityTimer();
		}

		/// <summary>
		/// Called when the user starts/stops scrolling the video destinations list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnDestinationsMovingChanged(object sender, BoolEventArgs args)
		{
			if (args.Data)
				StopVisibilityTimer();
			else
				ResetVisibilityTimer();
		}

		/// <summary>
		/// Called when the user presses one of the video destination buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnDestinationButtonPressed(object sender, UShortEventArgs args)
		{
			if (m_Source == null)
				return;

			MetlifeDestination destination;
			if (!m_Destinations.TryElementAt(args.Data, out destination))
				return;

			if (Room == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to toggle routing - room is null");
				return;
			}

			bool routed = GetDestinationSelected(destination);

			// Hack - null is used for the "Share with Call" item.
			if (destination == null)
			{
				if (routed)
					Room.Routing.UnrouteVideoFromCodec(m_Source);
				else
					Room.Routing.RouteVideoToCodec(m_Source);
			}
			else
			{
				if (routed)
					Room.Routing.UnrouteVideoFromDestinationOverride(m_Source, destination);
				else
					Room.Routing.RouteVideoToDestinationOverride(m_Source, destination);
			}

			ResetVisibilityTimer();
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			ResetVisibilityTimer();
		}

		#endregion
	}
}
