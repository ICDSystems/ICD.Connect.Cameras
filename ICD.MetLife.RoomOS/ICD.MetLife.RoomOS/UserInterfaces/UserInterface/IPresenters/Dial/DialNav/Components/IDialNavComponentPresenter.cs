using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial.DialNav;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial.DialNav.Components
{
	public interface IDialNavComponentPresenter : IPresenter<IDialNavComponentView>
	{
		/// <summary>
		/// Sets the visibility of the associated menu.
		/// </summary>
		/// <param name="show"></param>
		void ShowMenu(bool show);
	}
}
