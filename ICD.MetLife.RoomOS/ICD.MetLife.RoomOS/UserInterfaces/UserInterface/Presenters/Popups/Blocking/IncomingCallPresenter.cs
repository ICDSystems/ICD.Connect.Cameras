using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Blocking
{
	public sealed class IncomingCallPresenter : AbstractPresenter<IIncomingCallView>, IIncomingCallPresenter
	{
		private readonly List<IConferenceSource> m_Sources;
		private readonly SafeCriticalSection m_SourcesSection;

		/// <summary>
		/// Gets the number of incoming sources.
		/// </summary>
		private int SourceCount { get { return m_SourcesSection.Execute(() => m_Sources.Count); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public IncomingCallPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_Sources = new List<IConferenceSource>();
			m_SourcesSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			foreach (IConferenceSource source in m_Sources)
				Unsubscribe(source);
			m_Sources.Clear();
		}

		/// <summary>
		/// Refreshes the state of the UI.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IIncomingCallView view)
		{
			base.Refresh(view);

			IConferenceSource first = GetFirstSource();
			if (first == null)
			{
				ShowView(false);
				return;
			}

			string message = GetMessage(first);
			bool showCancel = first.Direction != eConferenceSourceDirection.Incoming;

			view.SetMessageText(message);
			view.ShowAcceptButton(!showCancel);
			view.ShowCancelButton(showCancel);
			view.ShowRejectButton(!showCancel);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the message string for the given source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private static string GetMessage(IConferenceSource source)
		{
			switch (source.Direction)
			{
				case eConferenceSourceDirection.Incoming:
				case eConferenceSourceDirection.Outgoing:
					return GetIncomingOutgoingMessage(source);

				case eConferenceSourceDirection.Undefined:
					return GetUndefinedMessage(source);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Gets the message for a source with undefined direction.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private static string GetUndefinedMessage(IConferenceSource source)
		{
			string typeString = StringUtils.NiceName(source.SourceType);
			string name = string.IsNullOrEmpty(source.Name) ? "Unknown" : source.Name;
			string number = string.IsNullOrEmpty(source.Number) ? "Unknown" : source.Number;

			return string.Format("Placing {0} Call\n{1}\n{2}", typeString, name, number);
		}

		/// <summary>
		/// Gets the incoming/outgoing message for the given source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private static string GetIncomingOutgoingMessage(IConferenceSource source)
		{
			string typeString = StringUtils.NiceName(source.SourceType);
			bool outgoing = source.Direction == eConferenceSourceDirection.Outgoing;
			string direction = outgoing ? "Outgoing" : "Incoming";
			string toFrom = outgoing ? "to" : "from";

			string name = string.IsNullOrEmpty(source.Name) ? "Unknown" : source.Name;
			string number = string.IsNullOrEmpty(source.Number) ? "Unknown" : source.Number;

			return string.Format("{0} {1} Call {2}\n{3}\n{4}", direction, typeString, toFrom, name, number);
		}

		/// <summary>
		/// Returns true if the source belongs in the incoming call alert.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private static bool IsValidIncomingSource(IConferenceSource source)
		{
			if (source == null)
				return false;

			switch (source.AnswerState)
			{
				case eConferenceSourceAnswerState.Answered:
				case eConferenceSourceAnswerState.Autoanswered:
				case eConferenceSourceAnswerState.Ignored:
					return false;
			}

			switch (source.Status)
			{
				case eConferenceSourceStatus.Undefined:
				case eConferenceSourceStatus.Connected:
				case eConferenceSourceStatus.Disconnecting:
				case eConferenceSourceStatus.OnHold:
				case eConferenceSourceStatus.EarlyMedia:
				case eConferenceSourceStatus.Preserved:
				case eConferenceSourceStatus.RemotePreserved:
				case eConferenceSourceStatus.Disconnected:
				case eConferenceSourceStatus.Idle:
					return false;

				case eConferenceSourceStatus.Dialing:
				case eConferenceSourceStatus.Ringing:
				case eConferenceSourceStatus.Connecting:
					return true;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Gets the first unanswered source.
		/// </summary>
		/// <returns></returns>
		private IConferenceSource GetFirstSource()
		{
			return m_SourcesSection.Execute(() => m_Sources.FirstOrDefault());
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
		/// Adds the source to the collection.
		/// </summary>
		/// <param name="source"></param>
		private void AddSource(IConferenceSource source)
		{
			m_SourcesSection.Enter();

			try
			{
				if (m_Sources.Contains(source))
					return;

				m_Sources.Add(source);
				Subscribe(source);
			}
			finally
			{
				m_SourcesSection.Leave();
			}

			ShowView(SourceCount > 0);
			RefreshIfVisible(false);
		}

		/// <summary>
		/// Removes the source from the collection.
		/// </summary>
		/// <param name="source"></param>
		private void RemoveSource(IConferenceSource source)
		{
			m_SourcesSection.Enter();
			try
			{
				if (!m_Sources.Contains(source))
					return;

				m_Sources.Remove(source);
				Unsubscribe(source);
			}
			finally
			{
				m_SourcesSection.Leave();
			}

			RefreshIfVisible();
			ShowView(SourceCount > 0);
		}

		/// <summary>
		/// Called when a new source is detected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnRecentSourceAdded(object sender, ConferenceSourceEventArgs args)
		{
			IConferenceSource source = args.Data;
			if (IsValidIncomingSource(source))
				AddSource(source);
		}

		#endregion

		#region Source Callbacks

		/// <summary>
		/// Subscribe to the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Subscribe(IConferenceSource source)
		{
			source.OnNameChanged += SourceOnNameChanged;
			source.OnNumberChanged += SourceOnNumberChanged;
			source.OnSourceTypeChanged += SourceOnSourceTypeChanged;
			source.OnStatusChanged += SourceOnStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Unsubscribe(IConferenceSource source)
		{
			source.OnNameChanged -= SourceOnNameChanged;
			source.OnNumberChanged -= SourceOnNumberChanged;
			source.OnSourceTypeChanged -= SourceOnSourceTypeChanged;
			source.OnStatusChanged -= SourceOnStatusChanged;
		}

		/// <summary>
		/// Called when the source status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SourceOnStatusChanged(object sender, ConferenceSourceStatusEventArgs args)
		{
			IConferenceSource source = sender as IConferenceSource;
			if (!IsValidIncomingSource(source))
				RemoveSource(source);
		}

		/// <summary>
		/// Called when the source type changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SourceOnSourceTypeChanged(object sender, EventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the source number changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SourceOnNumberChanged(object sender, StringEventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the source name changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SourceOnNameChanged(object sender, StringEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IIncomingCallView view)
		{
			base.Subscribe(view);

			view.OnAnswerButtonPressed += ViewOnAnswerButtonPressed;
			view.OnRejectButtonPressed += ViewOnRejectButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IIncomingCallView view)
		{
			base.Unsubscribe(view);

			view.OnAnswerButtonPressed -= ViewOnAnswerButtonPressed;
			view.OnRejectButtonPressed -= ViewOnRejectButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the answer button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnAnswerButtonPressed(object sender, EventArgs eventArgs)
		{
			IConferenceSource source = GetFirstSource();
			if (source != null)
				source.Answer();
		}

		/// <summary>
		/// Called when the user presses the reject button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnRejectButtonPressed(object sender, EventArgs eventArgs)
		{
			IConferenceSource source = GetFirstSource();
			if (source != null)
				source.Hangup();
		}

		#endregion
	}
}
