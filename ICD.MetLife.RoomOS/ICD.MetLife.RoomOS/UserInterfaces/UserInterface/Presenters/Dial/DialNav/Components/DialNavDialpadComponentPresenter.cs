using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial.DialNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial.DialNav.Components
{
	public sealed class DialNavDialpadComponentPresenter : AbstractDialNavComponentPresenter<IDialpadPresenter>,
	                                                       IDialNavDialpadComponentPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public DialNavDialpadComponentPresenter(int room, INavigationController nav, IViewFactory views,
		                                        ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Gets the label text for the component.
		/// </summary>
		/// <returns></returns>
		protected override string GetLabelText()
		{
			return "Dialpad";
		}

		/// <summary>
		/// Gets the icon for the component.
		/// </summary>
		/// <returns></returns>
		protected override IIcon GetIcon()
		{
			return GetView().GetIcon(eDialNavIcon.Dialpad);
		}
	}
}
