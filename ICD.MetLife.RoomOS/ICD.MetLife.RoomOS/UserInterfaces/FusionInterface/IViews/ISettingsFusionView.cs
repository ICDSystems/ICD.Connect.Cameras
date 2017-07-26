using System;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.FusionInterface.IViews
{
	public interface ISettingsFusionView : IFusionView
	{
		event EventHandler<StringEventArgs> OnRoomNumberChanged;
		event EventHandler<StringEventArgs> OnRoomNameChanged;
		event EventHandler<StringEventArgs> OnRoomTypeChanged;
		event EventHandler<StringEventArgs> OnRoomOwnerChanged;
		event EventHandler<StringEventArgs> OnRoomPhoneNumberChanged;
		event EventHandler<StringEventArgs> OnBuildingChanged;
		event EventHandler OnApplyRoomSettings;

		void SetRoomNumber(string number);

		void SetRoomName(string name);

		void SetRoomType(string type);

		void SetRoomOwner(string owner);

		void SetRoomPhoneNumber(string number);

		void SetBuilding(string building);
	}
}
