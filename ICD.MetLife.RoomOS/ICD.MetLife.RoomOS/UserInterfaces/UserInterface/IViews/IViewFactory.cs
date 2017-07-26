namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews
{
	/// <summary>
	/// IViewFactory provides functionality for a presenter to obtain its view.
	/// </summary>
	public interface IViewFactory
	{
		/// <summary>
		/// Instantiates a new view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T GetNewView<T>() where T : class, IView;
	}
}
