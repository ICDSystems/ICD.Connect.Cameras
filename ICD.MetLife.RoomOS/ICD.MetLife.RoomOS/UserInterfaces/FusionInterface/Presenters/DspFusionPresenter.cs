using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Presenters
{
	public sealed class DspFusionPresenter : AbstractFusionPresenter<IDspFusionView>, IDspFusionPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="presenters"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public DspFusionPresenter(int roomId, IFusionPresenterFactory presenters, IFusionViewFactory views, ICore core)
			: base(roomId, presenters, views, core)
		{
		}

		/// <summary>
		/// Refreshes the state of the view.
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();

			const bool hasAudioSystem = false; // todo
			string type = string.Empty; // todo
			string activeFaultStatus = string.Empty; // todo
			string hostName = string.Empty; // todo
			string defaultGateway = string.Empty; // todo
			string linkStatus = string.Empty; // todo
			string ipAddress = string.Empty; // todo
			string subnetMask = string.Empty; // todo
			string registrationStatus = string.Empty; // todo
			string macAddress = string.Empty; // todo

			GetView().SetHasAudioSystem(hasAudioSystem);
			GetView().SetDspType(type);
			GetView().SetDspActiveFaultStatus(activeFaultStatus);
			GetView().SetDspHostName(hostName);
			GetView().SetDspDefaultGateway(defaultGateway);
			GetView().SetDspLinkStatus(linkStatus);
			GetView().SetDspIpAddress(ipAddress);
			GetView().SetDspSubnetMask(subnetMask);
			GetView().SetVoipRegistrationStatus(registrationStatus);
			GetView().SetVoipMacAddress(macAddress);
		}
	}
}
