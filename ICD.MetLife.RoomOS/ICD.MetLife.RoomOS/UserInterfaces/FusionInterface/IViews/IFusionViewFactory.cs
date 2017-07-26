namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface IFusionViewFactory
	{
		/// <summary>
		/// Instantiates a new view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T GetNewView<T>() where T : class, IFusionView;
	}
}
