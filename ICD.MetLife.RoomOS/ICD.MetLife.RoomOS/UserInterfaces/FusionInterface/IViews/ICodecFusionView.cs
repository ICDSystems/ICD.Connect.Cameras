namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface ICodecFusionView : IFusionView
	{
		void SetHasVideoConferencing(bool hasVideoConferencing);

		void SetVideoConferencingOnline(bool videoConferencingOnline);

		void SetVtcCameraOnline(bool online);

		void SetVtcPresentationSwitcherOnline(bool online);

		void SetVtcSystemName(string name);

		void SetVtcIpAddress(string address);

		void SetVtcDefaultGateway(string gateway);

		void SetVtcSubnetMask(string mask);

		void SetVtcGatekeeperStatus(string status);

		void SetVtcGatekeeperMode(string mode);

		void SetVtcGatekeeperAddress(string address);

		void SetVtcH323Id(string id);

		void SetVtcE164Alias(string alias);

		void SetVtcSipUri(string uri);

		void SetVtcSipProxyAddress(string address);

		void SetVtcSipProxyStatus(string status);

		void SetVtcSoftwareVersion(string version);
	}
}
