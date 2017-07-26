using ICD.Connect.Analytics.FusionPro;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class DspFusionView : AbstractFusionView, IDspFusionView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public DspFusionView(IFusionRoom fusionRoom)
			: base(fusionRoom)
		{
		}

		public void SetHasAudioSystem(bool hasAudioSystem)
		{
			m_HasAudioSystemInput.SendValue(hasAudioSystem);
		}

		public void SetDspType(string type)
		{
			m_DspTypeInput.SendValue(type);
		}

		public void SetDspActiveFaultStatus(string status)
		{
			m_DspActiveFaultStatusInput.SendValue(status);
		}

		public void SetDspHostName(string name)
		{
			m_DspHostNameInput.SendValue(name);
		}

		public void SetDspDefaultGateway(string gateway)
		{
			m_DspDefaultGatewayInput.SendValue(gateway);
		}

		public void SetDspLinkStatus(string status)
		{
			m_DspLinkStatusInput.SendValue(status);
		}

		public void SetDspIpAddress(string address)
		{
			m_DspIpAddressInput.SendValue(address);
		}

		public void SetDspSubnetMask(string mask)
		{
			m_DspSubnetMaskInput.SendValue(mask);
		}

		public void SetVoipRegistrationStatus(string status)
		{
			m_VoipRegistrationStatusInput.SendValue(status);
		}

		public void SetVoipMacAddress(string address)
		{
			m_VoipMacAddressInput.SendValue(address);
		}
	}
}
