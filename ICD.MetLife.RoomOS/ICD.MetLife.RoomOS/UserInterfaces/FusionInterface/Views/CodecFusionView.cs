using ICD.Connect.Analytics.FusionPro;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class CodecFusionView : AbstractFusionView, ICodecFusionView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public CodecFusionView(IFusionRoom fusionRoom)
			: base(fusionRoom)
		{
		}

		#region Methods

		public void SetHasVideoConferencing(bool hasVideoConferencing)
		{
			m_VtcHasVideoConferencingInput.SendValue(hasVideoConferencing);
		}

		public void SetVideoConferencingOnline(bool videoConferencingOnline)
		{
			m_VtcVideoConferencingOnlineInput.SendValue(videoConferencingOnline);
		}

		public void SetVtcCameraOnline(bool online)
		{
			m_VtcCameraOnlineInput.SendValue(online);
		}

		public void SetVtcPresentationSwitcherOnline(bool online)
		{
			m_VtcPresentationSwitcherOnlineInput.SendValue(online);
		}

		public void SetVtcSystemName(string name)
		{
			m_VtcSystemNameInput.SendValue(name);
		}

		public void SetVtcIpAddress(string address)
		{
			m_VtcIpAddressInput.SendValue(address);
		}

		public void SetVtcDefaultGateway(string gateway)
		{
			m_VtcDefaultGatewayInput.SendValue(gateway);
		}

		public void SetVtcSubnetMask(string mask)
		{
			m_VtcSubnetMaskInput.SendValue(mask);
		}

		public void SetVtcGatekeeperStatus(string status)
		{
			m_VtcGatekeeperStatusInput.SendValue(status);
		}

		public void SetVtcGatekeeperMode(string mode)
		{
			m_VtcGatekeeperModeInput.SendValue(mode);
		}

		public void SetVtcGatekeeperAddress(string address)
		{
			m_VtcGatekeeperAddressInput.SendValue(address);
		}

		public void SetVtcH323Id(string id)
		{
			m_VtcH323IdInput.SendValue(id);
		}

		public void SetVtcE164Alias(string alias)
		{
			m_VtcE164AliasInput.SendValue(alias);
		}

		public void SetVtcSipUri(string uri)
		{
			m_VtcSipUriInput.SendValue(uri);
		}

		public void SetVtcSipProxyAddress(string address)
		{
			m_VtcSipProxyAddressInput.SendValue(address);
		}

		public void SetVtcSipProxyStatus(string status)
		{
			m_VtcSipProxyStatusInput.SendValue(status);
		}

		public void SetVtcSoftwareVersion(string version)
		{
			m_VtcSoftwareVersionInput.SendValue(version);
		}

		#endregion
	}
}
