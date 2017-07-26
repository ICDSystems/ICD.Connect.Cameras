using System;
using System.Collections.Generic;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner.NavSource;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner
{
	public interface ITvTunerView : INavSourceView
	{
		/// <summary>
		/// Raised when the user presses the channel up button.
		/// </summary>
		event EventHandler OnChannelUpPressed;

		/// <summary>
		/// Raised when the user presses the channel down button.
		/// </summary>
		event EventHandler OnChannelDownPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IChannelPresetView> GetChildCallViews(IViewFactory factory, ushort count);
	}
}
