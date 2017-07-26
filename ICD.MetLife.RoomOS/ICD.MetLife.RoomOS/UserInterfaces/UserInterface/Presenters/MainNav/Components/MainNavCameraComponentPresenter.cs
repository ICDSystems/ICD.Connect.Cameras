using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Camera;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.MainNav.Components
{
	public sealed class MainNavCameraComponentPresenter : AbstractMainNavMenuComponentPresenter<ICameraPresenter>,
	                                                      IMainNavCameraComponentPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public MainNavCameraComponentPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Gets the label text for the component.
		/// </summary>
		/// <returns></returns>
		protected override string GetLabel()
		{
			return "Camera";
		}

		/// <summary>
		/// Gets the icon for the component.
		/// </summary>
		/// <returns></returns>
		protected override IIcon GetIcon()
		{
			return GetView().GetIcon(eMainNavIcon.Camera);
		}
	}
}
