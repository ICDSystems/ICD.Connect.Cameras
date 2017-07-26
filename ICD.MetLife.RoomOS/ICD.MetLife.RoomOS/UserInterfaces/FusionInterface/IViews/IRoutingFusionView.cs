namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface IRoutingFusionView : IFusionView
	{
		void SetFrontInputAvailable(bool available);

		void SetFrontInputSync(bool online);

		void SetWirelessInputSync(bool online);

		void SetFrontInputVideoType(string type);

		void SetFrontInputHdmiResolution(string resolution);

		void SetFrontInputVgaResolution(string resolution);

		void SetRearInputVideoType(string type);

		void SetRearInputHdmiResolution(string resolution);

		void SetRearInputVgaResolution(string resolution);

		void SetVideoConferencingMonitor1Sync(bool sync);

		void SetTvTunerSync(bool sync);

		void SetFrontTransmitterType(string type);

		void SetFrontTransmitterFirmwareVersion(string version);

		void SetRearTransmitterType(string type);

		void SetRearTransmitterFirmwareVersion(string version);
	}
}
