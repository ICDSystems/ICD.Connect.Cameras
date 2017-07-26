using System.Collections.Generic;
using ICD.Connect.Analytics.FusionPro;
using ICD.Connect.Panels.SigIo;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class DspFusionView
	{
		private DigitalSigInputOutput m_HasAudioSystemInput;
		private SerialSigInputOutput m_DspTypeInput;
		private SerialSigInputOutput m_DspActiveFaultStatusInput;
		private SerialSigInputOutput m_DspHostNameInput;
		private SerialSigInputOutput m_DspDefaultGatewayInput;
		private SerialSigInputOutput m_DspLinkStatusInput;
		private SerialSigInputOutput m_DspIpAddressInput;
		private SerialSigInputOutput m_DspSubnetMaskInput;
		private SerialSigInputOutput m_VoipRegistrationStatusInput;
		private SerialSigInputOutput m_VoipMacAddressInput;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected override void InstantiateControls(IFusionRoom fusionRoom)
		{
			m_HasAudioSystemInput = new DigitalSigInputOutput(fusionRoom, 51);
			m_DspTypeInput = new SerialSigInputOutput(fusionRoom, 73);
			m_DspActiveFaultStatusInput = new SerialSigInputOutput(fusionRoom, 74);
			m_DspHostNameInput = new SerialSigInputOutput(fusionRoom, 75);
			m_DspDefaultGatewayInput = new SerialSigInputOutput(fusionRoom, 76);
			m_DspLinkStatusInput = new SerialSigInputOutput(fusionRoom, 77);
			m_DspIpAddressInput = new SerialSigInputOutput(fusionRoom, 78);
			m_DspSubnetMaskInput = new SerialSigInputOutput(fusionRoom, 79);
			m_VoipRegistrationStatusInput = new SerialSigInputOutput(fusionRoom, 109);
			m_VoipMacAddressInput = new SerialSigInputOutput(fusionRoom, 110);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<AbstractSigOutput> GetChildren()
		{
			yield return m_HasAudioSystemInput;
			yield return m_DspTypeInput;
			yield return m_DspActiveFaultStatusInput;
			yield return m_DspHostNameInput;
			yield return m_DspDefaultGatewayInput;
			yield return m_DspLinkStatusInput;
			yield return m_DspIpAddressInput;
			yield return m_DspSubnetMaskInput;
			yield return m_VoipRegistrationStatusInput;
			yield return m_VoipMacAddressInput;
		}
	}
}
