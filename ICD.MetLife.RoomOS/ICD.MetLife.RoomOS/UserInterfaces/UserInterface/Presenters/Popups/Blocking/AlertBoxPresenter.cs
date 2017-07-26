using System.Collections.Generic;
using System.Linq;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Blocking
{
	public sealed class AlertBoxPresenter : AbstractPresenter<IAlertBoxView>, IAlertBoxPresenter
	{
		private readonly Queue<Alert> m_Alerts;
		private readonly SafeCriticalSection m_AlertsSection;

		/// <summary>
		/// Gets the current alert.
		/// </summary>
		public Alert Current { get { return m_AlertsSection.Execute(() => m_Alerts.FirstOrDefault()); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public AlertBoxPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_Alerts = new Queue<Alert>();
			m_AlertsSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IAlertBoxView view)
		{
			base.Refresh(view);

			Alert current = Current;

			string title = current == null ? string.Empty : current.Title;
			string message = current == null ? string.Empty : current.Message;
			AlertOption[] options = current == null ? new AlertOption[0] : current.Options;

			message = string.Format("{0}{1}{2}", title, HtmlUtils.NEWLINE, message);

			view.SetMessage(message);
			view.SetButtonLabels(options.Select(o => o.Name));
		}

		/// <summary>
		/// Enqueues the alert to be displayed.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <param name="options"></param>
		public void Enqueue(string title, string message, params AlertOption[] options)
		{
			Enqueue(new Alert(title, message, options));
		}

		/// <summary>
		/// Enqueues the alert to be displayed.
		/// </summary>
		/// <param name="alert"></param>
		public void Enqueue(Alert alert)
		{
			ServiceProvider.GetService<ILoggerService>().AddEntry(eSeverity.Alert, string.Format("AlertBox: {0} - {1}", alert.Title, alert.Message));

			m_AlertsSection.Enter();

			try
			{
				// Prevents the same alert being queued multiple times.
				bool isQueued = m_Alerts.Any(a => CompareAlerts(a, alert));
				if (!isQueued)
					m_Alerts.Enqueue(alert);
			}
			finally
			{
				m_AlertsSection.Leave();
			}

			RefreshIfVisible();
			ShowView(true);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Dequeues the current alert and updates the view.
		/// </summary>
		/// <returns></returns>
		private Alert Dequeue()
		{
			Alert output = null;
			int count;

			m_AlertsSection.Enter();

			try
			{
				count = m_Alerts.Count;
				if (count > 0)
					output = m_Alerts.Dequeue();
			}
			finally
			{
				m_AlertsSection.Leave();
			}

			ShowView(count > 1);
			RefreshIfVisible();

			return output;
		}

		/// <summary>
		/// Returns true if the alerts have the same title and message.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static bool CompareAlerts(Alert a, Alert b)
		{
			if (a == null && b == null)
				return true;

			if (a == null || b == null)
				return false;

			return a.Title == b.Title && a.Message == b.Message;
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IAlertBoxView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IAlertBoxView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when a button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnButtonPressed(object sender, UShortEventArgs args)
		{
			Alert alert = Dequeue();
			if (alert == null)
				return;

			AlertOption option = alert.Options[args.Data];
			option.Callback();
		}

		#endregion
	}
}
