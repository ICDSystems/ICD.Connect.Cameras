using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters
{
	/// <summary>
	/// A presenter that represents a single item in a list.
	/// 
	/// Components are often recycled between use so we don't want to automatically take views from the factory.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AbstractComponentPresenter<T> : AbstractPresenter<T>
		where T : class, IView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractComponentPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public override sealed void Refresh()
		{
			T view = GetView(false);

			// Don't refresh if we currently have no view.
			if (view != null)
				Refresh(view);
		}

		/// <summary>
		/// Sets the view.
		/// </summary>
		/// <param name="view"></param>
		public override sealed void SetView(T view)
		{
			if (view == GetView(false))
				return;

			// Special case - Can't set an SRL count to 0 (yay) so hide when cleaning up items.
			if (view == null && GetView(false) != null)
				ShowView(false);

			base.SetView(view);
		}
	}
}
