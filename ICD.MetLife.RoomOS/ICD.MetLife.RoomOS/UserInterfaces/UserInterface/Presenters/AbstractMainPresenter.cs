using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Common;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters
{
	/// <summary>
	/// AbstractMainPresenter shows the PageMainPresenter when visible.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AbstractMainPresenter<T> : AbstractPresenter<T>
		where T : class, IView
	{
		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected abstract string Title { get; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractMainPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (!args.Data)
				return;

			Navigation.NavigateTo<IPageMainPresenter>().SetMenu(this, Title);
		}
	}
}
