using System;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.MainNav;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.MainNav.Components
{
	/// <summary>
	/// Base class for nav bar items.
	/// </summary>
	public abstract class AbstractMainNavComponentPresenter : AbstractComponentPresenter<IMainNavComponentView>,
	                                                          IMainNavComponentPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractMainNavComponentPresenter(int room, INavigationController nav,
		                                            IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IMainNavComponentView view)
		{
			base.Refresh(view);

			IIcon icon = GetIcon();
			eIconState state = GetIconState();
			string label = GetLabel();

			view.SetIcon(icon, state);
			view.SetLabelText(label);
		}

		#region Protected Methods

		/// <summary>
		/// Gets the label text for the component.
		/// </summary>
		/// <returns></returns>
		protected abstract string GetLabel();

		/// <summary>
		/// Gets the current icon state for the component.
		/// </summary>
		/// <returns></returns>
		protected abstract eIconState GetIconState();

		protected abstract IIcon GetIcon();

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IMainNavComponentView view)
		{
			base.Subscribe(view);

			view.OnPressed += ViewOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IMainNavComponentView view)
		{
			base.Unsubscribe(view);

			view.OnPressed -= ViewOnPressed;
		}

		/// <summary>
		/// Called when the component is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected abstract void ViewOnPressed(object sender, EventArgs eventArgs);

		#endregion
	}
}
