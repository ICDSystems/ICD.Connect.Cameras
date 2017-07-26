namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface ILightingFusionView : IFusionView
	{
		void SetLightingProcessorOnline(bool online);

		void SetRoomHasShades(bool shades);

		void SetShadesProcessorOnline(bool online);
	}
}
