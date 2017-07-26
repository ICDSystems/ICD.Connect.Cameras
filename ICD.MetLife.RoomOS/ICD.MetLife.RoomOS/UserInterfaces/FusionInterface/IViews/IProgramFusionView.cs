namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface IProgramFusionView : IFusionView
	{
		void SetProgramFile(string file);

		void SetSystemName(string name);

		void SetCompiledDate(string date);

		void SetProgramSlot(string slot);

		void SetProgramVersion(string version);

		void SetProcessorModel(string model);

		void SetProcessorFirmwareVersion(string version);

		void SetProcessorAddress(string address);
	}
}
