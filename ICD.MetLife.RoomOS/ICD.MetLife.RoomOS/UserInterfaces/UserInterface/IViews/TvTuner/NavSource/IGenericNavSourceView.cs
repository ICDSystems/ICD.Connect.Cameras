namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner.NavSource
{
	public interface IGenericNavSourceView : INavSourceView
	{
		/// <summary>
		/// Sets the Watching, Playing, etc portion of the stop button.
		/// </summary>
		/// <param name="verb"></param>
		void SetSourcePresentParticiple(string verb);

		/// <summary>
		/// Sets the name of the source currently routed to the display.
		/// </summary>
		/// <param name="name"></param>
		void SetSourceName(string name);
	}
}
