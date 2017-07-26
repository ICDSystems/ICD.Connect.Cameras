using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public sealed class RecentsPresenter : AbstractDialPresenter<IRecentsView>, IRecentsPresenter
	{
		private readonly RecentsComponentPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		private IConferenceSource m_Selected;

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Recent Calls"; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public RecentsPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_ChildrenFactory = new RecentsComponentPresenterFactory(nav, ItemFactory);
			m_RefreshSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			UnsubscribeChildren();
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IRecentsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				UnsubscribeChildren();

				IEnumerable<IConferenceSource> sources =
					Room == null
						? Enumerable.Empty<IConferenceSource>()
						: Room.ConferenceManager
						      .GetRecentSources()
						      .Reverse()
						      .Distinct();

				foreach (IRecentCallPresenter presenter in m_ChildrenFactory.BuildChildren(sources))
				{
					Subscribe(presenter);

					presenter.Selected = presenter.Source == m_Selected;
					presenter.ShowView(true);
				}

				RefreshDialButton();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Sets the given source as selected.
		/// </summary>
		/// <param name="source"></param>
		public void SetSelected(IConferenceSource source)
		{
			if (source == m_Selected)
				return;

			// We can't select it if it doesn't exist in the list.
			if (source != null && m_ChildrenFactory.All(p => p.Source != source))
				return;

			m_Selected = source;

			foreach (IRecentCallPresenter presenter in m_ChildrenFactory)
				presenter.Selected = presenter.Source == source;

			RefreshDialButton();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sets the enabled state and label of the dial button.
		/// </summary>
		private void RefreshDialButton()
		{
			eConferenceSourceType type = m_Selected == null ? eConferenceSourceType.Unknown : m_Selected.SourceType;
			string callTypeString = StringUtils.NiceName(type);

			GetView().SetCallTypeText(callTypeString);
			GetView().EnabledDialButton(m_Selected != null);
		}

		/// <summary>
		/// Unsubscribes from all of the child presenters.
		/// </summary>
		private void UnsubscribeChildren()
		{
			foreach (IRecentCallPresenter presenter in m_ChildrenFactory)
				Unsubscribe(presenter);
		}

		/// <summary>
		/// Generates the given number of views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IRecentCallView> ItemFactory(ushort count)
		{
			return GetView().GetChildCallViews(ViewFactory, count);
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Subscribe to the recent call presenter callbacks.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(IRecentCallPresenter presenter)
		{
			presenter.OnSelectedStateChanged += PresenterOnSelectedStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the recent call presenter callbacks.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(IRecentCallPresenter presenter)
		{
			presenter.OnSelectedStateChanged -= PresenterOnSelectedStateChanged;
		}

		/// <summary>
		/// Called when a presenter selection state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void PresenterOnSelectedStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			IRecentCallPresenter recentCallPresenter = sender as IRecentCallPresenter;
			if (recentCallPresenter == null)
				return;

			IConferenceSource source = recentCallPresenter.Source;

			if (boolEventArgs.Data)
				SetSelected(source);
			else if (source == m_Selected)
				SetSelected(null);
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

			room.ConferenceManager.OnRecentSourceAdded += ConferenceManagerOnRecentSourceAdded;
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

			room.ConferenceManager.OnRecentSourceAdded -= ConferenceManagerOnRecentSourceAdded;
		}

		/// <summary>
		/// Called when a new call source is added to the conference manager.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnRecentSourceAdded(object sender, ConferenceSourceEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IRecentsView view)
		{
			base.Subscribe(view);

			view.OnDialButtonPressed += ViewOnDialButtonPressed;
			view.OnRemoveFromListButtonPressed += ViewOnRemoveFromListButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IRecentsView view)
		{
			base.Unsubscribe(view);

			view.OnDialButtonPressed -= ViewOnDialButtonPressed;
			view.OnRemoveFromListButtonPressed -= ViewOnRemoveFromListButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the remove from list button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnRemoveFromListButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Called when the user presses the dial button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDialButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_Selected != null)
				Dial(m_Selected.Number);
			m_Selected = null;
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			m_Selected = null;

			base.ViewOnVisibilityChanged(sender, args);
		}

		#endregion
	}
}
