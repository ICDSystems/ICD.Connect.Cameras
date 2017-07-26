using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface IRoomFusionView : IFusionView
	{
		event EventHandler OnUpdateContacts;

		void SetRoomOccupied(bool occupied);

		void SetVoiceConferencingDialPlan(string dialPlan);
	}
}
