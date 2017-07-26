using System;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial.DialNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial.DialNav;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial.DialNav
{
	public sealed class DialNavHomeButtonPresenter : AbstractPresenter<IDialNavHomeButtonView>, IDialNavHomeButtonPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public DialNavHomeButtonPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IDialNavHomeButtonView view)
		{
			base.Subscribe(view);

			view.OnPressed += ViewOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IDialNavHomeButtonView view)
		{
			base.Unsubscribe(view);

			view.OnPressed -= ViewOnPressed;
		}

		/// <summary>
		/// Called when the home button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			Navigation.NavigateTo<IMainNavPresenter>();
		}

		#endregion
	}
}
