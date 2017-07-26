namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Common
{
	public interface IPageMainPresenter : IPresenter
	{
		/// <summary>
		/// Sets the current menu for the presenter.
		/// </summary>
		/// <param name="menu"></param>
		/// <param name="title"></param>
		void SetMenu(IPresenter menu, string title);
	}
}
