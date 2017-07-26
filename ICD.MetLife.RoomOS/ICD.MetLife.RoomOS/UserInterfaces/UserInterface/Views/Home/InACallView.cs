using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Home
{
	public sealed partial class InACallView : AbstractView, IInACallView
	{
		private readonly List<ICallStatusView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public InACallView(ISigInputOutput panel)
			: base(panel)
		{
			m_ChildList = new List<ICallStatusView>();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<ICallStatusView> GetChildCallViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_CallStatusList, m_ChildList, count);
		}
	}
}
