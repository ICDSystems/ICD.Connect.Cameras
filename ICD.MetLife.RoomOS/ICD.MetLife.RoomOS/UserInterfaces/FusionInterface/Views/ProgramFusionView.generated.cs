using System.Collections.Generic;
using ICD.Connect.Panels.SigIo;
using ICD.Connect.Analytics.FusionPro;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class ProgramFusionView
	{
		private SerialSigInputOutput m_ProgramFileInput;
		private SerialSigInputOutput m_SystemNameInput;
		private SerialSigInputOutput m_CompiledDateInput;
		private SerialSigInputOutput m_ProgramSlotInput;
		private SerialSigInputOutput m_ProgramVersionInput;
		private SerialSigInputOutput m_ProcessorModelInput;
		private SerialSigInputOutput m_ProcessorFirmwareVersionInput;
		private SerialSigInputOutput m_ProcessorAddressInput;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="fusionRoom"></param>
		protected override void InstantiateControls(IFusionRoom fusionRoom)
		{
			m_ProgramFileInput = new SerialSigInputOutput(fusionRoom, 55);
			m_SystemNameInput = new SerialSigInputOutput(fusionRoom, 56);
			m_CompiledDateInput = new SerialSigInputOutput(fusionRoom, 57);
			m_ProgramSlotInput = new SerialSigInputOutput(fusionRoom, 58);
			m_ProgramVersionInput = new SerialSigInputOutput(fusionRoom, 59);
			m_ProcessorModelInput = new SerialSigInputOutput(fusionRoom, 60);
			m_ProcessorFirmwareVersionInput = new SerialSigInputOutput(fusionRoom, 62);
			m_ProcessorAddressInput = new SerialSigInputOutput(fusionRoom, 61);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<AbstractSigOutput> GetChildren()
		{
			yield return m_ProgramFileInput;
			yield return m_SystemNameInput;
			yield return m_CompiledDateInput;
			yield return m_ProgramSlotInput;
			yield return m_ProgramVersionInput;
			yield return m_ProcessorModelInput;
			yield return m_ProcessorFirmwareVersionInput;
			yield return m_ProcessorAddressInput;
		}
	}
}

