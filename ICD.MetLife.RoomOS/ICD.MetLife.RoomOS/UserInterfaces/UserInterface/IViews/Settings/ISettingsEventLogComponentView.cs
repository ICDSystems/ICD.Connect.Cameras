namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings
{
	public interface ISettingsEventLogComponentView : IView
	{
		/// <summary>
		/// Sets the index label for the log item.
		/// </summary>
		/// <param name="index"></param>
		void SetIndexLabel(uint index);

		/// <summary>
		/// Sets the message label for the log item.
		/// </summary>
		/// <param name="message"></param>
		void SetMessageLabel(string message);
	}
}
