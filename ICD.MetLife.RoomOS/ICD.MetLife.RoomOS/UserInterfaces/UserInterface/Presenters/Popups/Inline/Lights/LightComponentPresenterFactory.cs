using ICD.Connect.Lighting;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Lights;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Lights
{
	public sealed class LightComponentPresenterFactory
		: AbstractListItemFactory<LightingProcessorControl, ILightComponentPresenter, ILightComponentView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public LightComponentPresenterFactory(INavigationController navigationController,
		                                      ListItemFactory<ILightComponentView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(LightingProcessorControl model, ILightComponentPresenter presenter,
		                                     ILightComponentView view)
		{
			presenter.SetView(view);
			presenter.Control = model;
		}
	}
}
