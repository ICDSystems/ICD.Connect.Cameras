using System.Collections.Generic;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.MainNav
{
	/// <summary>
	/// Represents the horizontal row of navigation items along the bottom of the panel.
	/// </summary>
	public interface IMainNavView : IView
	{
		/// <summary>
		/// Sets the visibility of the child at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		void SetChildVisible(ushort index, bool visible);

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IMainNavComponentView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
