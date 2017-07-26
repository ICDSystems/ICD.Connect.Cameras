using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Services.Logging;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.Endpoints.Sources;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.ShareMenu;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.ShareMenu;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.ShareMenu
{
	public sealed class ShareMenuPresenter : AbstractMainPresenter<IShareMenuView>, IShareMenuPresenter
	{
		private readonly ShareComponentPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		#region Properties

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Share"; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public ShareMenuPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_ChildrenFactory = new ShareComponentPresenterFactory(nav, ItemFactory);
			m_RefreshSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			m_ChildrenFactory.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IShareMenuView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<MetlifeSource> sources = GetOnlineSources();
				foreach (IShareComponentPresenter presenter in m_ChildrenFactory.BuildChildren(sources))
					presenter.ShowView(true);

				RefreshStopSharingButton();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Gets the sources that are available in the share menu.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MetlifeSource> GetOnlineSources()
		{
			if (Room == null)
				return Enumerable.Empty<MetlifeSource>();

			return Room.Routing
			           .GetOnlineSources()
			           .Where(s => s.SourceFlags.HasFlag(eSourceFlags.Share));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Refreshes the enabled state of the stop sharing button.
		/// </summary>
		private void RefreshStopSharingButton()
		{
			bool enable = Room != null &&
			              GetOnlineSources().Any(s => Room.Routing.GetActiveDestinations(s, eConnectionType.Video, false).Any());
			GetView().EnableStopSharingButton(enable);
		}

		/// <summary>
		/// Generates the given number of child views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IShareComponentView> ItemFactory(ushort count)
		{
			return GetView().GetChildCallViews(ViewFactory, count);
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
			if (IsViewVisible)
				RefreshStopSharingButton();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IShareMenuView view)
		{
			base.Subscribe(view);

			view.OnStopSharingButtonPressed += ViewOnStopSharingButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IShareMenuView view)
		{
			base.Unsubscribe(view);

			view.OnStopSharingButtonPressed -= ViewOnStopSharingButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the stop sharing button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnStopSharingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to stop sharing - room is null");
				return;
			}

			if (Room.GetDevice<CiscoCodec>() != null)
				Room.StopPresentation();

			Room.Routing.UnrouteSources();
		}

		#endregion
	}
}
