using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public abstract class AbstractDirectoryComponentPresenter : AbstractFavoritesAndDirectoryComponentPresenter,
	                                                            IDirectoryComponentPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractDirectoryComponentPresenter(int room, INavigationController nav,
		                                              IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}
	}
}
