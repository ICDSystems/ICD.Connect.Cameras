using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsEventLogView : AbstractView, ISettingsEventLogView
	{
		private readonly List<ISettingsEventLogComponentView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public SettingsEventLogView(ISigInputOutput panel)
			: base(panel)
		{
			m_ChildList = new List<ISettingsEventLogComponentView>();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<ISettingsEventLogComponentView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_List, m_ChildList, count);
		}
	}
}
