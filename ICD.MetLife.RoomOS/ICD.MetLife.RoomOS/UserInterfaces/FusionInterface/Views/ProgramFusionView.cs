using ICD.Connect.Analytics.FusionPro;
using ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.Views
{
	public sealed partial class ProgramFusionView : AbstractFusionView, IProgramFusionView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fusionRoom"></param>
		public ProgramFusionView(IFusionRoom fusionRoom)
			: base(fusionRoom)
		{
		}

		public void SetProgramFile(string file)
		{
			m_ProgramFileInput.SendValue(file);
		}

		public void SetSystemName(string name)
		{
			m_SystemNameInput.SendValue(name);
		}

		public void SetCompiledDate(string date)
		{
			m_CompiledDateInput.SendValue(date);
		}

		public void SetProgramSlot(string slot)
		{
			m_ProgramSlotInput.SendValue(slot);
		}

		public void SetProgramVersion(string version)
		{
			m_ProgramVersionInput.SendValue(version);
		}

		public void SetProcessorModel(string model)
		{
			m_ProcessorModelInput.SendValue(model);
		}

		public void SetProcessorFirmwareVersion(string version)
		{
			m_ProcessorFirmwareVersionInput.SendValue(version);
		}

		public void SetProcessorAddress(string address)
		{
			m_ProcessorAddressInput.SendValue(address);
		}
	}
}
