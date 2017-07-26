using System;
using ICD.Common.EventArguments;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;
using ICD.Connect.Conferencing.ConferenceSources;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial
{
	public interface IRecentCallPresenter : IPresenter<IRecentCallView>
	{
		/// <summary>
		/// Raised when the item becomes selected/deselected.
		/// </summary>
		event EventHandler<BoolEventArgs> OnSelectedStateChanged;

		/// <summary>
		/// Gets/sets the property source.
		/// </summary>
		IConferenceSource Source { get; set; }

		/// <summary>
		/// Gets/sets the selected state of the source.
		/// </summary>
		bool Selected { get; set; }
	}
}
