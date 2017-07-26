using System.Collections.Generic;
using ICD.MetLife.RoomOS.Endpoints.Sources;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.ShareMenu
{
	public interface IShareMenuPresenter : IPresenter
	{
		/// <summary>
		/// Gets the sources that are available in the share menu.
		/// </summary>
		/// <returns></returns>
		IEnumerable<MetlifeSource> GetOnlineSources();
	}
}
