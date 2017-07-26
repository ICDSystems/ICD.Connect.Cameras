using System;
using ICD.Connect.TvPresets;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner
{
	public interface IChannelPresetPresenter : IPresenter<IChannelPresetView>
	{
		/// <summary>
		/// Raised when the user presses the channel preset.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Gets/sets the station.
		/// </summary>
		Station Station { get; set; }
	}
}
