using System.Collections.Generic;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial.DialNav
{
	public interface IDialNavView : IView
	{
		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IDialNavComponentView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
