using System;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial.DialNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.MainNav.Components
{
	public sealed class MainNavContactsComponentPresenter : AbstractMainNavMenuComponentPresenter<IDirectoryPresenter>,
	                                                        IMainNavContactsComponentPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public MainNavContactsComponentPresenter(int room, INavigationController nav, IViewFactory views,
		                                         ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Gets the label text for the component.
		/// </summary>
		/// <returns></returns>
		protected override string GetLabel()
		{
			return "Contacts";
		}

		/// <summary>
		/// Gets the icon for the component.
		/// </summary>
		/// <returns></returns>
		protected override IIcon GetIcon()
		{
			return GetView().GetIcon(eMainNavIcon.Contacts);
		}

		/// <summary>
		/// Called when the view is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			base.ViewOnPressed(sender, eventArgs);

			Navigation.NavigateTo<IDialNavPresenter>();
		}
	}
}
