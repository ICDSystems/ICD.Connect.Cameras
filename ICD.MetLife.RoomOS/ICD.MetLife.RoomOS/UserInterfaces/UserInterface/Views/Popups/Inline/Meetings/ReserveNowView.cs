using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Connect.UI.Widgets;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Meetings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Meetings
{
	public sealed partial class ReserveNowView : AbstractView, IReserveNowView
	{
		public event EventHandler OnReserveButtonPressed;
		public event EventHandler OnCancelButtonPressed;
		public event EventHandler<DateTimeEventArgs> OnSelectedTimeChanged;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="panel"></param>
		public ReserveNowView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		public override void Dispose()
		{
			OnReserveButtonPressed = null;
			OnCancelButtonPressed = null;
			OnSelectedTimeChanged = null;

			base.Dispose();
		}

		/// <summary>
		/// Set the text of the "next meeting time" serial join. 
		/// Should be either "until HH:MM AM/PM (xxxx)" where xxxx is TimeSpan.ToReadableString()
		/// or "for the rest of the day" if no more meetings.
		/// </summary>
		/// <param name="text"></param>
		public void SetNextMeetingTime(string text)
		{
			m_NextMeetingLabel.SetLabelTextAtJoin(m_NextMeetingLabel.SerialLabelJoins.First(), text);
		}

		public void SetReserveButtonEnabled(bool enabled)
		{
			m_ReserveButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the hours, minutes and AM/PM currently displayed.
		/// </summary>
		/// <param name="time"></param>
		public void SetSelectedTime(DateTime time)
		{
			m_ClockWidget.SetTime(time);
		}

		#endregion

		#region Private Methods

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ReserveButton.OnPressed += ReserveButtonOnOnReleased;
			m_CancelButton.OnPressed += CancelButtonOnOnReleased;
			m_ClockWidget.OnSelectedTimeChanged += ClockWidgetOnSelectedTimeChanged;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ReserveButton.OnPressed -= ReserveButtonOnOnReleased;
			m_CancelButton.OnPressed -= CancelButtonOnOnReleased;
			m_ClockWidget.OnSelectedTimeChanged -= ClockWidgetOnSelectedTimeChanged;
		}

		private void CancelButtonOnOnReleased(object sender, EventArgs eventArgs)
		{
			OnCancelButtonPressed.Raise(this);
		}

		private void ReserveButtonOnOnReleased(object sender, EventArgs eventArgs)
		{
			OnReserveButtonPressed.Raise(this);
		}

		private void ClockWidgetOnSelectedTimeChanged(SpinnerListClockWidget sender, DateTime time)
		{
			OnSelectedTimeChanged.Raise(this, new DateTimeEventArgs(time));
		}

		#endregion
	}
}
