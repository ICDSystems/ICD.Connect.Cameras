namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews
{
	public interface ISoundsView : IView
	{
		/// <summary>
		/// Starts/stops the ringtone sound.
		/// </summary>
		/// <param name="playing"></param>
		void PlayRingtone(bool playing);
	}
}
