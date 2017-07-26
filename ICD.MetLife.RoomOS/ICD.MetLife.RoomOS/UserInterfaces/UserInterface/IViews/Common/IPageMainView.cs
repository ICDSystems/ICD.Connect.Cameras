namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Common
{
	/// <summary>
	/// The background for man-nav subpages.
	/// </summary>
	public interface IPageMainView : IView
	{
		/// <summary>
		/// Sets the title label text.
		/// </summary>
		/// <param name="title"></param>
		void SetPageTitle(string title);
	}
}
