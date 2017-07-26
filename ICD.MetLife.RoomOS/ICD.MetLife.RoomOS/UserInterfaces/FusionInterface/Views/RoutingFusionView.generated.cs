using System.Collections.Generic;
using ICD.Connect.Panels.SigIo;
using ICD.Connect.Analytics.FusionPro;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class RoutingFusionView
	{
		private DigitalSigInputOutput m_FrontInputAvailableInput;
		private DigitalSigInputOutput m_FrontInputSyncInput;
		private DigitalSigInputOutput m_WirelessInputSyncInput;

		private SerialSigInputOutput m_FrontInputVideoTypeInput;
		private SerialSigInputOutput m_FrontInputHdmiResolutionInput;
		private SerialSigInputOutput m_FrontInputVgaResolutionInput;

		private SerialSigInputOutput m_RearInputVideoTypeInput;
		private SerialSigInputOutput m_RearInputHdmiResolutionInput;
		private SerialSigInputOutput m_RearInputVgaResolutionInput;

		private DigitalSigInputOutput m_VideoConferencingMonitor1SyncInput;
		private DigitalSigInputOutput m_TvTunerSyncInput;

		private SerialSigInputOutput m_FrontTransmitterTypeInput;
		private SerialSigInputOutput m_FrontTransmitterFirmwareVersionInput;
		private SerialSigInputOutput m_RearTransmitterTypeInput;
		private SerialSigInputOutput m_RearTransmitterFirmwareVersionInput;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected override void InstantiateControls(IFusionRoom fusionRoom)
		{
			m_FrontInputAvailableInput = new DigitalSigInputOutput(fusionRoom, 53);
			m_FrontInputSyncInput = new DigitalSigInputOutput(fusionRoom, 64);
			m_WirelessInputSyncInput = new DigitalSigInputOutput(fusionRoom, 66);

			m_FrontInputVideoTypeInput = new SerialSigInputOutput(fusionRoom, 100);
			m_FrontInputHdmiResolutionInput = new SerialSigInputOutput(fusionRoom, 101);
			m_FrontInputVgaResolutionInput = new SerialSigInputOutput(fusionRoom, 102);

			m_RearInputVideoTypeInput = new SerialSigInputOutput(fusionRoom, 103);
			m_RearInputHdmiResolutionInput = new SerialSigInputOutput(fusionRoom, 104);
			m_RearInputVgaResolutionInput = new SerialSigInputOutput(fusionRoom, 105);

			m_VideoConferencingMonitor1SyncInput = new DigitalSigInputOutput(fusionRoom, 67);
			m_TvTunerSyncInput = new DigitalSigInputOutput(fusionRoom, 69);

			m_FrontTransmitterTypeInput = new SerialSigInputOutput(fusionRoom, 63);
			m_FrontTransmitterFirmwareVersionInput = new SerialSigInputOutput(fusionRoom, 64);
			m_RearTransmitterTypeInput = new SerialSigInputOutput(fusionRoom, 65);
			m_RearTransmitterFirmwareVersionInput = new SerialSigInputOutput(fusionRoom, 66);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<AbstractSigOutput> GetChildren()
		{
			yield return m_FrontInputAvailableInput;
			yield return m_FrontInputSyncInput;
			yield return m_WirelessInputSyncInput;
			yield return m_FrontInputVideoTypeInput;
			yield return m_FrontInputHdmiResolutionInput;
			yield return m_FrontInputVgaResolutionInput;
			yield return m_RearInputVideoTypeInput;
			yield return m_RearInputHdmiResolutionInput;
			yield return m_RearInputVgaResolutionInput;
			yield return m_VideoConferencingMonitor1SyncInput;
			yield return m_TvTunerSyncInput;
			yield return m_FrontTransmitterTypeInput;
			yield return m_FrontTransmitterFirmwareVersionInput;
			yield return m_RearTransmitterTypeInput;
			yield return m_RearTransmitterFirmwareVersionInput;
		}
	}
}
