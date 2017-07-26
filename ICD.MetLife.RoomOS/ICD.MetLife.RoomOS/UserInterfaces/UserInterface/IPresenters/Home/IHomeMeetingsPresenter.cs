using System;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Home
{
	public interface IHomeMeetingsPresenter : IPresenter
	{
		/// <summary>
		/// Raised when a meeting is added, or all meetings are removed from the subpage.
		/// </summary>
		event EventHandler<BoolEventArgs> OnHasVisibleMeetingsChanged;

		/// <summary>
		/// Returns true if there are meetings currently being displayed.
		/// </summary>
		bool HasVisibleMeetings { get; }
	}
}
