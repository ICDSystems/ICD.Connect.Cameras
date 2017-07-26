using System;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.ShareMenu;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.MainNav.Components
{
	public sealed class MainNavShareComponentPresenter : AbstractMainNavMenuComponentPresenter<IShareMenuPresenter>,
	                                                     IMainNavShareComponentPresenter
	{
		private IAlertBoxPresenter m_AlertBox;

		/// <summary>
		/// Gets the alert box menu.
		/// </summary>
		private IAlertBoxPresenter AlertBox
		{
			get { return m_AlertBox ?? (m_AlertBox = Navigation.LazyLoadPresenter<IAlertBoxPresenter>()); }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public MainNavShareComponentPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Gets the label text for the component.
		/// </summary>
		/// <returns></returns>
		protected override string GetLabel()
		{
			return "Share";
		}

		/// <summary>
		/// Gets the icon for the component.
		/// </summary>
		/// <returns></returns>
		protected override IIcon GetIcon()
		{
			return GetView().GetIcon(eMainNavIcon.Share);
		}

		/// <summary>
		/// Called when the component is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			if (!Menu.IsViewVisible && !Menu.GetOnlineSources().Any())
			{
				AlertBox.Enqueue("No Source Available", "No sources are detected by the system", new AlertOption("Close"));
				return;
			}
			
			base.ViewOnPressed(sender, eventArgs);
		}
	}
}
