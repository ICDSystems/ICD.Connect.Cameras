using ICD.Connect.Devices.Controls;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Volume;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Volume;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Volume
{
	public sealed class VolumeComponentPresenterFactory
		: AbstractListItemFactory<IVolumeDeviceControl, IVolumeComponentPresenter, IVolumeComponentView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public VolumeComponentPresenterFactory(INavigationController navigationController,
		                                       ListItemFactory<IVolumeComponentView> viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IVolumeDeviceControl model, IVolumeComponentPresenter presenter,
		                                     IVolumeComponentView view)
		{
			presenter.SetView(view);
			presenter.VolumeControl = model;
		}
	}
}
