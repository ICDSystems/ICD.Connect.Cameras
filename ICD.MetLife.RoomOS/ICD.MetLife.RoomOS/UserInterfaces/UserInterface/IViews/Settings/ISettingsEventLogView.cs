using System.Collections.Generic;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings
{
	public interface ISettingsEventLogView : IView
	{
		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<ISettingsEventLogComponentView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
