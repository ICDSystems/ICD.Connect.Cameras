using System;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Analytics.FusionPro;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class LoggingFusionView : AbstractFusionView, ILoggingFusionView
	{
		public event EventHandler<UShortEventArgs> OnLoggingSeverityLevelChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public LoggingFusionView(IFusionRoom fusionRoom)
			: base(fusionRoom)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnLoggingSeverityLevelChanged = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the logging severity level.
		/// </summary>
		/// <param name="level"></param>
		public void SetLoggingSeverityLevel(ushort level)
		{
			m_LoggingSeverityInputOutput.SendValue(level);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribe to the control events.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_LoggingSeverityInputOutput.OnOutput += FusionRoomSeverityLevelChanged;
		}

		/// <summary>
		/// Unsubscribe from the control events.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_LoggingSeverityInputOutput.OnOutput -= FusionRoomSeverityLevelChanged;
		}

		/// <summary>
		/// Called when fusion changes the severity level.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void FusionRoomSeverityLevelChanged(object parent, UShortEventArgs args)
		{
			OnLoggingSeverityLevelChanged.Raise(this, new UShortEventArgs(args.Data));
		}

		#endregion
	}
}
