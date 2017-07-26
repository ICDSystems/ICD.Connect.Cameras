namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home
{
	public interface IRoomNumbersView : IView
	{
		/// <summary>
		/// Sets the audio number text.
		/// </summary>
		/// <param name="number"></param>
		void SetAudioNumber(string number);

		/// <summary>
		/// Sets the video number text.
		/// </summary>
		/// <param name="number"></param>
		void SetVideoNumber(string number);

		/// <summary>
		/// Sets the visibility of the audio label.
		/// </summary>
		/// <param name="show"></param>
		void ShowAudioLabel(bool show);

		/// <summary>
		/// Sets the visibility of the video label.
		/// </summary>
		/// <param name="show"></param>
		void ShowVideoLabel(bool show);

		/// <summary>
		/// Sets the sharing status text (e.g. "Watching TV").
		/// </summary>
		/// <param name="sharingStatus"></param>
		void SetSharingStatusText(string sharingStatus);
	}
}
