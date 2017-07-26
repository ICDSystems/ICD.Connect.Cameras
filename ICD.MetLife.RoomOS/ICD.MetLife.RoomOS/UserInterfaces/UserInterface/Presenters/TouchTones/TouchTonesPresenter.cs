using System.Collections.Generic;
using System.Linq;
using ICD.Common.EventArguments;
using ICD.Connect.Settings.Core;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TouchTones;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TouchTones;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.TouchTones
{
	public sealed class TouchTonesPresenter : AbstractMainPresenter<ITouchTonesView>, ITouchTonesPresenter
	{
		private IConferenceSource m_Selected;
		private IConferenceSource[] m_Sources;

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Touchtones"; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public TouchTonesPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_Sources = new IConferenceSource[0];
		}

		#region Methods

		/// <summary>
		/// Sets the given source as selected.
		/// </summary>
		/// <param name="source"></param>
		public void SetSelected(IConferenceSource source)
		{
			if (source == m_Selected)
				return;

			m_Selected = source;

			RefreshIfVisible();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ITouchTonesView view)
		{
			base.Refresh(view);

			m_Sources = GetSources().ToArray();

			view.SetContactNames(m_Sources.Select(s => s.Name));
			view.SetContactsButtonsVisible(m_Sources.Length > 0);

			for (ushort index = 0; index < m_Sources.Length; index++)
				view.SetContactSelected(index, m_Sources[index] == m_Selected);
		}

		/// <summary>
		/// Gets the online sources.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConferenceSource> GetSources()
		{
			IConference conference = Room == null ? null : Room.ConferenceManager.ActiveConference;
			return conference == null
				       ? Enumerable.Empty<IConferenceSource>()
				       : conference.GetSources().Where(s => s.Status == eConferenceSourceStatus.Connected);
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

			room.ConferenceManager.OnRecentSourceAdded += DialingPlanOnRecentSourceAdded;
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

			room.ConferenceManager.OnRecentSourceAdded -= DialingPlanOnRecentSourceAdded;
		}

		/// <summary>
		/// Called when a source is added to the conference.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void DialingPlanOnRecentSourceAdded(object sender, ConferenceSourceEventArgs eventArgs)
		{
			RefreshIfVisible();

			// Select the most recent source.
			SetSelected(eventArgs.Data);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ITouchTonesView view)
		{
			base.Subscribe(view);

			view.OnContactButtonPressed += ViewOnContactButtonPressed;
			view.OnToneButtonPressed += ViewOnToneButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ITouchTonesView view)
		{
			base.Unsubscribe(view);

			view.OnContactButtonPressed -= ViewOnContactButtonPressed;
			view.OnToneButtonPressed -= ViewOnToneButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a tone button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="charEventArgs"></param>
		private void ViewOnToneButtonPressed(object sender, CharEventArgs charEventArgs)
		{
			if (m_Selected != null)
				m_Selected.SendDtmf(charEventArgs.Data);
		}

		/// <summary>
		/// Called when the user presses a contact button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="uShortEventArgs"></param>
		private void ViewOnContactButtonPressed(object sender, UShortEventArgs uShortEventArgs)
		{
			IConferenceSource source = m_Sources[uShortEventArgs.Data];
			SetSelected(source);
		}

		#endregion
	}
}
