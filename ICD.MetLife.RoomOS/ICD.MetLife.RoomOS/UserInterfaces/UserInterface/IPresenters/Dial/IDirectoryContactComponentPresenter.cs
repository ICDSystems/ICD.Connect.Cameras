using ICD.Connect.Conferencing.Contacts;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial
{
	public interface IDirectoryContactComponentPresenter : IDirectoryComponentPresenter
	{
		IContact Contact { get; set; }
	}
}
