using System.Collections.Generic;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home
{
	/// <summary>
	/// The InACallView lists the current active calls taking place.
	/// </summary>
	public interface IInACallView : IView
	{
		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<ICallStatusView> GetChildCallViews(IViewFactory factory, ushort count);
	}
}
