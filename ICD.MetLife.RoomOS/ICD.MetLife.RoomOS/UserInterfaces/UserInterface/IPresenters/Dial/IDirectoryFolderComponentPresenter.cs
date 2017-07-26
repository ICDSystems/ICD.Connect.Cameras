using ICD.Connect.Conferencing.Cisco.Components.Directory.Tree;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial
{
	public interface IDirectoryFolderComponentPresenter : IDirectoryComponentPresenter
	{
		IFolder Folder { get; set; }
	}
}
