using System;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Home
{
	public sealed class CallStatusPresenter : AbstractComponentPresenter<ICallStatusView>, ICallStatusPresenter
	{
		private readonly SafeTimer m_Heartbeat;
		private IConferenceSource m_Source;

		/// <summary>
		/// Sets the conference source for this presenter.
		/// </summary>
		/// <value></value>
		public IConferenceSource ConferenceSource
		{
			get { return m_Source; }
			set
			{
				if (value == m_Source)
					return;

				Unsubscribe(m_Source);
				m_Source = value;
				Subscribe(m_Source);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public CallStatusPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_Heartbeat = new SafeTimer(HeartbeatCallback, 1000, 1000);
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_Heartbeat.Dispose();

			Unsubscribe(m_Source);

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ICallStatusView view)
		{
			base.Refresh(view);

			// Type and status
			string statusString = null;
			eColor color = eColor.Default;
			if (m_Source != null)
			{
				color = GetColorForStatus(m_Source.Status);

				string status = StringUtils.NiceName(m_Source.Status);
				string type = StringUtils.NiceName(m_Source.SourceType);
				statusString = string.Format("{0} - {1}", type, status);
			}

			// Name/number
			string name = m_Source == null ? string.Empty : m_Source.Name;
			if (string.IsNullOrEmpty(name))
				name = m_Source == null ? string.Empty : m_Source.Number;

			// Duration
			ushort duration = GetDuration();

			view.SetCallStatusText(statusString ?? string.Empty, color);
			view.SetCallName(name ?? string.Empty);
			view.SetCallDuration(duration);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the duration of the source in seconds.
		/// </summary>
		/// <returns></returns>
		private ushort GetDuration()
		{
			return m_Source == null ? (ushort)0 : (ushort)m_Source.GetDuration().TotalSeconds;
		}

		/// <summary>
		/// Gets the color for the given source status.
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		private static eColor GetColorForStatus(eConferenceSourceStatus status)
		{
			return status == eConferenceSourceStatus.OnHold ? eColor.Yellow : eColor.Default;
		}

		/// <summary>
		/// Updates the call duration.
		/// </summary>
		private void HeartbeatCallback()
		{
			if (m_Source == null || !IsViewVisible)
				return;

			ushort duration = GetDuration();
			GetView().SetCallDuration(duration);
		}

		#endregion

		#region Source Callbacks

		/// <summary>
		/// Subscribe to the source events.
		/// </summary>
		/// <param name="source"></param>
		private void Subscribe(IConferenceSource source)
		{
			if (source == null)
				return;

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
			if (source == null)
				return;

			source.OnNameChanged -= SourceOnNameChanged;
			source.OnNumberChanged -= SourceOnNumberChanged;
			source.OnSourceTypeChanged -= SourceOnSourceTypeChanged;
			source.OnStatusChanged -= SourceOnStatusChanged;
		}

		private void SourceOnStatusChanged(object sender, ConferenceSourceStatusEventArgs args)
		{
			RefreshIfVisible();
		}

		private void SourceOnSourceTypeChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		private void SourceOnNumberChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		private void SourceOnNameChanged(object sender, StringEventArgs stringEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ICallStatusView view)
		{
			base.Subscribe(view);

			view.OnEndCallButtonPressed += ViewOnEndCallButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ICallStatusView view)
		{
			base.Unsubscribe(view);

			view.OnEndCallButtonPressed -= ViewOnEndCallButtonPressed;
		}

		/// <summary>
		/// Called when the end call button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnEndCallButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_Source != null)
				m_Source.Hangup();
		}

		#endregion
	}
}
