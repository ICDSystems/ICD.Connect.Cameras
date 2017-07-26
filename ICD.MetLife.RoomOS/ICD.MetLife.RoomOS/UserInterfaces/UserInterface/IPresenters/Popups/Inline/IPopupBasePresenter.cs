namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline
{
	public interface IPopupBasePresenter : IPresenter
	{
		/// <summary>
		/// Sets the current menu for the presenter.
		/// </summary>
		/// <param name="menu"></param>
		/// <param name="title"></param>
		void SetMenu(IPresenter menu, string title);
	}
}
