using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class RackFusionPresenter : AbstractFusionPresenter<IRackFusionView>, IRackFusionPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public RackFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
			: base(roomId, presenters, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();

			string ipAddress = string.Empty; // todo
			string temperature = string.Empty; // todo
			string peakVolts = string.Empty; // todo
			string rmsVolts = string.Empty; // todo
			string peakAmps = string.Empty; // todo
			string rmsAmps = string.Empty; // todo

			GetView().SetRacklinkIpAddress(ipAddress);
			GetView().SetRackTemperature(temperature);
			GetView().SetRackPeakVolts(peakVolts);
			GetView().SetRackRmsVolts(rmsVolts);
			GetView().SetRackPeakAmps(peakAmps);
			GetView().SetRackRmsAmps(rmsAmps);
		}
	}
}
