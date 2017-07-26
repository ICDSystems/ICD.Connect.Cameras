namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface IDisplaysFusionView : IFusionView
	{
		void SetLeftDisplayOnline(bool online);

		void SetLeftDisplayReceiverOnline(bool online);

		void SetLeftDisplayReceiverType(string type);

		void SetLeftDisplayReceiverFirmwareVersion(string version);

		void SetRightDisplayReceiverOnline(bool online);

		void SetRightDisplayReceiverType(string type);

		void SetRightDisplayReceiverFirmwareVersion(string version);
	}
}
