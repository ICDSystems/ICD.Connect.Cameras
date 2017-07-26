using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial.DialNav;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial.DialNav
{
	public sealed partial class DialNavView : AbstractView, IDialNavView
	{
		private readonly List<IDialNavComponentView> m_ChildViews;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public DialNavView(ISigInputOutput panel)
			: base(panel)
		{
			m_ChildViews = new List<IDialNavComponentView>();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IDialNavComponentView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_List, m_ChildViews, count);
		}
	}
}
