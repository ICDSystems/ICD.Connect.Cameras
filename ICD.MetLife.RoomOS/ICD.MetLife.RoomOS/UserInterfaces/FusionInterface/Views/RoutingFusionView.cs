using ICD.Connect.Analytics.FusionPro;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class RoutingFusionView : AbstractFusionView, IRoutingFusionView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public RoutingFusionView(IFusionRoom fusionRoom)
			: base(fusionRoom)
		{
		}

		public void SetFrontInputAvailable(bool available)
		{
			m_FrontInputAvailableInput.SendValue(available);
		}

		public void SetFrontInputSync(bool online)
		{
			m_FrontInputSyncInput.SendValue(online);
		}

		public void SetWirelessInputSync(bool online)
		{
			m_WirelessInputSyncInput.SendValue(online);
		}

		public void SetFrontInputVideoType(string type)
		{
			m_FrontInputVideoTypeInput.SendValue(type);
		}

		public void SetFrontInputHdmiResolution(string resolution)
		{
			m_FrontInputHdmiResolutionInput.SendValue(resolution);
		}

		public void SetFrontInputVgaResolution(string resolution)
		{
			m_FrontInputVgaResolutionInput.SendValue(resolution);
		}

		public void SetRearInputVideoType(string type)
		{
			m_RearInputVideoTypeInput.SendValue(type);
		}

		public void SetRearInputHdmiResolution(string resolution)
		{
			m_RearInputHdmiResolutionInput.SendValue(resolution);
		}

		public void SetRearInputVgaResolution(string resolution)
		{
			m_RearInputVgaResolutionInput.SendValue(resolution);
		}

		public void SetVideoConferencingMonitor1Sync(bool sync)
		{
			m_VideoConferencingMonitor1SyncInput.SendValue(sync);
		}

		public void SetTvTunerSync(bool sync)
		{
			m_TvTunerSyncInput.SendValue(sync);
		}

		public void SetFrontTransmitterType(string type)
		{
			m_FrontTransmitterTypeInput.SendValue(type);
		}

		public void SetFrontTransmitterFirmwareVersion(string version)
		{
			m_FrontTransmitterFirmwareVersionInput.SendValue(version);
		}

		public void SetRearTransmitterType(string type)
		{
			m_RearTransmitterTypeInput.SendValue(type);
		}

		public void SetRearTransmitterFirmwareVersion(string version)
		{
			m_RearTransmitterFirmwareVersionInput.SendValue(version);
		}
	}
}
