using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Volume
{
	public interface IVolumeView : IView
	{
		/// <summary>
		/// Raised when the list starts/stops scrolling.
		/// </summary>
		event EventHandler<BoolEventArgs> OnScrollingChanged;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IVolumeComponentView> GetChildVolumeViews(IViewFactory factory, ushort count);
	}
}
