using ICD.MetLife.RoomOS.Endpoints.Sources;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner.NavSource
{
	public interface INavSourcePresenter : IPresenter
	{
		MetlifeSource Source { get; set; }
	}
}
