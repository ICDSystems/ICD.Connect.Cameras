using ICD.Common.EventArguments;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Connect.Settings.Core;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class LoggingFusionPresenter : AbstractFusionPresenter<ILoggingFusionView>, ILoggingFusionPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public LoggingFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
			: base(roomId, presenters, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();

			GetView().SetLoggingSeverityLevel((ushort)ServiceProvider.GetService<ILoggerService>().SeverityLevel);
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(MetlifeRoom room)
		{
			base.Subscribe(room);

			ServiceProvider.GetService<ILoggerService>().OnSeverityLevelChanged += LoggingCoreOnSeverityLevelChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			ServiceProvider.GetService<ILoggerService>().OnSeverityLevelChanged -= LoggingCoreOnSeverityLevelChanged;
		}

		/// <summary>
		/// Called when the logging core changes severity level.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LoggingCoreOnSeverityLevelChanged(object sender, SeverityEventArgs args)
		{
			RefreshAsync();
		}

		#endregion

		#region View Calbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ILoggingFusionView view)
		{
			base.Subscribe(view);

			view.OnLoggingSeverityLevelChanged += ViewOnLoggingSeverityLevelChanged;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ILoggingFusionView view)
		{
			base.Unsubscribe(view);

			view.OnLoggingSeverityLevelChanged -= ViewOnLoggingSeverityLevelChanged;
		}

		/// <summary>
		/// Called when fusion changes the logging severity level.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static void ViewOnLoggingSeverityLevelChanged(object sender, UShortEventArgs args)
		{
			ServiceProvider.GetService<ILoggerService>().SeverityLevel = (eSeverity)args.Data;
		}

		#endregion
	}
}
