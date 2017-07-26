namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface IPanelFusionView : IFusionView
	{
		void SetPanelOnline(bool online);

		void SetPanelType(string type);

		void SetPanelFirmwareVersion(string version);

		void SetPanelHeaderImagePath(string path);

		void SetPanelBackgroundImagePath(string path);
	}
}
