using ICD.MetLife.RoomOS.Endpoints.Sources;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Share
{
	public interface IDisplaySelectPresenter : IPresenter
	{
		/// <summary>
		/// Sets the source to be sent to selected displays.
		/// </summary>
		/// <param name="source"></param>
		void SetSource(MetlifeSource source);
	}
}
