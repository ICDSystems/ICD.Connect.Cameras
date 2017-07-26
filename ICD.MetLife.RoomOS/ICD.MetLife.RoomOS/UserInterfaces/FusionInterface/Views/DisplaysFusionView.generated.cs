using System.Collections.Generic;
using ICD.Connect.Panels.SigIo;
using ICD.Connect.Analytics.FusionPro;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class DisplaysFusionView
	{
		private DigitalSigInputOutput m_LeftDisplayOnlineInput;
		private DigitalSigInputOutput m_LeftDisplayReceiverOnlineInput;
		private SerialSigInputOutput m_LeftDisplayReceiverTypeInput;
		private SerialSigInputOutput m_LeftDisplayReceiverFirmwareVersionInput;

		private DigitalSigInputOutput m_RightDisplayReceiverOnlineInput;
		private SerialSigInputOutput m_RightDisplayReceiverTypeInput;
		private SerialSigInputOutput m_RightDisplayReceiverFirmwareVersionInput;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected override void InstantiateControls(IFusionRoom fusionRoom)
		{
			m_LeftDisplayOnlineInput = new DigitalSigInputOutput(fusionRoom, 55);
			m_LeftDisplayReceiverOnlineInput = new DigitalSigInputOutput(fusionRoom, 57);
			m_LeftDisplayReceiverTypeInput = new SerialSigInputOutput(fusionRoom, 67);
			m_LeftDisplayReceiverFirmwareVersionInput = new SerialSigInputOutput(fusionRoom, 68);

			m_RightDisplayReceiverOnlineInput = new DigitalSigInputOutput(fusionRoom, 58);
			m_RightDisplayReceiverTypeInput = new SerialSigInputOutput(fusionRoom, 69);
			m_RightDisplayReceiverFirmwareVersionInput = new SerialSigInputOutput(fusionRoom, 70);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<AbstractSigOutput> GetChildren()
		{
			yield return m_LeftDisplayOnlineInput;
			yield return m_LeftDisplayReceiverOnlineInput;
			yield return m_LeftDisplayReceiverTypeInput;
			yield return m_LeftDisplayReceiverFirmwareVersionInput;
			yield return m_RightDisplayReceiverOnlineInput;
			yield return m_RightDisplayReceiverTypeInput;
			yield return m_RightDisplayReceiverFirmwareVersionInput;
		}
	}
}
