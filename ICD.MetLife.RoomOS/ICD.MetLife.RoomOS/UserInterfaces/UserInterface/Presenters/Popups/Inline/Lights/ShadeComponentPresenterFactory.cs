using ICD.Connect.Lighting;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Lights;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Lights
{
	public sealed class ShadeComponentPresenterFactory
		: AbstractListItemFactory<LightingProcessorControl, IShadeComponentPresenter, IShadeComponentView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public ShadeComponentPresenterFactory(INavigationController navigationController,
		                                      ListItemFactory<IShadeComponentView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(LightingProcessorControl model, IShadeComponentPresenter presenter,
		                                     IShadeComponentView view)
		{
			presenter.SetView(view);
			presenter.Control = model;
		}
	}
}
