using System.Collections.Generic;
using ICD.Connect.Analytics.FusionPro;
using ICD.Connect.Panels.SigIo;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class CodecFusionView
	{
		private DigitalSigInputOutput m_VtcHasVideoConferencingInput;
		private DigitalSigInputOutput m_VtcVideoConferencingOnlineInput;
		private DigitalSigInputOutput m_VtcCameraOnlineInput;
		private DigitalSigInputOutput m_VtcPresentationSwitcherOnlineInput;
		private SerialSigInputOutput m_VtcSystemNameInput;
		private SerialSigInputOutput m_VtcIpAddressInput;
		private SerialSigInputOutput m_VtcDefaultGatewayInput;
		private SerialSigInputOutput m_VtcSubnetMaskInput;
		private SerialSigInputOutput m_VtcGatekeeperStatusInput;
		private SerialSigInputOutput m_VtcGatekeeperModeInput;
		private SerialSigInputOutput m_VtcGatekeeperAddressInput;
		private SerialSigInputOutput m_VtcH323IdInput;
		private SerialSigInputOutput m_VtcE164AliasInput;
		private SerialSigInputOutput m_VtcSipUriInput;
		private SerialSigInputOutput m_VtcSipProxyAddressInput;
		private SerialSigInputOutput m_VtcSipProxyStatusInput;
		private SerialSigInputOutput m_VtcSoftwareVersionInput;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected override void InstantiateControls(IFusionRoom fusionRoom)
		{
			m_VtcHasVideoConferencingInput = new DigitalSigInputOutput(fusionRoom, 59);
			m_VtcVideoConferencingOnlineInput = new DigitalSigInputOutput(fusionRoom, 52);
			m_VtcCameraOnlineInput = new DigitalSigInputOutput(fusionRoom, 74);
			m_VtcPresentationSwitcherOnlineInput = new DigitalSigInputOutput(fusionRoom, 50);
			m_VtcSystemNameInput = new SerialSigInputOutput(fusionRoom, 81);
			m_VtcIpAddressInput = new SerialSigInputOutput(fusionRoom, 82);
			m_VtcDefaultGatewayInput = new SerialSigInputOutput(fusionRoom, 83);
			m_VtcSubnetMaskInput = new SerialSigInputOutput(fusionRoom, 84);
			m_VtcGatekeeperStatusInput = new SerialSigInputOutput(fusionRoom, 85);
			m_VtcGatekeeperModeInput = new SerialSigInputOutput(fusionRoom, 86);
			m_VtcGatekeeperAddressInput = new SerialSigInputOutput(fusionRoom, 87);
			m_VtcH323IdInput = new SerialSigInputOutput(fusionRoom, 88);
			m_VtcE164AliasInput = new SerialSigInputOutput(fusionRoom, 89);
			m_VtcSipUriInput = new SerialSigInputOutput(fusionRoom, 90);
			m_VtcSipProxyAddressInput = new SerialSigInputOutput(fusionRoom, 91);
			m_VtcSipProxyStatusInput = new SerialSigInputOutput(fusionRoom, 92);
			m_VtcSoftwareVersionInput = new SerialSigInputOutput(fusionRoom, 93);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<AbstractSigOutput> GetChildren()
		{
			yield return m_VtcHasVideoConferencingInput;
			yield return m_VtcVideoConferencingOnlineInput;
			yield return m_VtcCameraOnlineInput;
			yield return m_VtcPresentationSwitcherOnlineInput;
			yield return m_VtcSystemNameInput;
			yield return m_VtcIpAddressInput;
			yield return m_VtcDefaultGatewayInput;
			yield return m_VtcSubnetMaskInput;
			yield return m_VtcGatekeeperStatusInput;
			yield return m_VtcGatekeeperModeInput;
			yield return m_VtcGatekeeperAddressInput;
			yield return m_VtcH323IdInput;
			yield return m_VtcE164AliasInput;
			yield return m_VtcSipUriInput;
			yield return m_VtcSipProxyAddressInput;
			yield return m_VtcSipProxyStatusInput;
			yield return m_VtcSoftwareVersionInput;
		}
	}
}
