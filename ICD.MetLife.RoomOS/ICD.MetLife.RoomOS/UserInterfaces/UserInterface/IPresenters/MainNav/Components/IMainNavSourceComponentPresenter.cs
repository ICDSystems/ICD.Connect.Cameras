using ICD.MetLife.RoomOS.Endpoints.Sources;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav.Components
{
	public interface IMainNavSourceComponentPresenter : IMainNavComponentPresenter
	{
		MetlifeSource Source { get; set; }
	}
}
