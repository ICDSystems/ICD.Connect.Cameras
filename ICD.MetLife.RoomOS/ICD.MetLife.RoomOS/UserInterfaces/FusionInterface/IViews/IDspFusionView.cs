namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface IDspFusionView : IFusionView
	{
		void SetHasAudioSystem(bool hasAudioSystem);

		void SetDspType(string type);

		void SetDspActiveFaultStatus(string status);

		void SetDspHostName(string name);

		void SetDspDefaultGateway(string gateway);

		void SetDspLinkStatus(string status);

		void SetDspIpAddress(string address);

		void SetDspSubnetMask(string mask);

		void SetVoipRegistrationStatus(string status);

		void SetVoipMacAddress(string address);
	}
}
