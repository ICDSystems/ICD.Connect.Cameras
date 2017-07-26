using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner
{
	public interface IChannelPresetView : IView
	{
		/// <summary>
		/// Raised when the user presses the preset.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Sets the icon image by path.
		/// </summary>
		/// <param name="path"></param>
		void SetImage(string path);
	}
}
