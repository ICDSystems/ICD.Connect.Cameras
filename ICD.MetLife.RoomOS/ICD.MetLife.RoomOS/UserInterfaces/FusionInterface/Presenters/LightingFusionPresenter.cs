using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class LightingFusionPresenter : AbstractFusionPresenter<ILightingFusionView>, ILightingFusionPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public LightingFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
			: base(roomId, presenters, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();

			const bool lightingProcessorOnline = false; // todo
			const bool roomHasShades = false; // todo
			const bool shadesProcessOnline = false; // todo

			GetView().SetLightingProcessorOnline(lightingProcessorOnline);
			GetView().SetRoomHasShades(roomHasShades);
			GetView().SetShadesProcessorOnline(shadesProcessOnline);
		}
	}
}
