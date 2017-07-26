using System;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface ILoggingFusionView : IFusionView
	{
		/// <summary>
		/// Raised when the logging severity level is changed.
		/// </summary>
		event EventHandler<UShortEventArgs> OnLoggingSeverityLevelChanged;

		/// <summary>
		/// Sets the logging severity level.
		/// </summary>
		/// <param name="level"></param>
		void SetLoggingSeverityLevel(ushort level);
	}
}
