using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.ShareMenu;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.ShareMenu
{
	public interface IShareComponentPresenter : IPresenter<IShareComponentView>
	{
		/// <summary>
		/// Gets/sets the source.
		/// </summary>
		MetlifeSource Source { get; set; }
	}
}
