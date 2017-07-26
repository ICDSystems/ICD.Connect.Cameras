using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.MainNav;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.MainNav
{
	public sealed partial class MainNavView : AbstractView, IMainNavView
	{
		private readonly List<IMainNavComponentView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public MainNavView(ISigInputOutput panel)
			: base(panel)
		{
			m_ChildList = new List<IMainNavComponentView>();
		}

		/// <summary>
		/// Sets the visibility of the child at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetChildVisible(ushort index, bool visible)
		{
			m_NavComponentList.SetItemVisible(index, visible);
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IMainNavComponentView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_NavComponentList, m_ChildList, count);
		}
	}
}
