namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews
{
	public interface IIcon
	{
		string GetIconString(eIconState state);
	}

	public interface IDialNavIcon : IIcon
	{
	}

	public interface IMainNavIcon : IIcon
	{
	}

	public interface ISourceIcon : IIcon
	{
	}

	public enum eIconState
	{
		Default,
		Active
	}

	public enum eRecentCallIconMode
	{
		Video,
		Audio,
		Folder,
		User
	}

	public enum eMainNavIcon
	{
		PrivacyMute,
		HoldCall,
		EndCall,
		Share,
		WatchTv,
		PlayGame,
		Camera,
		Touchtones,
		AddCall,
		Dial,
		Contacts,
		Meetings,
		Layout
	}

	public enum eDialNavIcon
	{
		Dialpad,
		Keyboard,
		RecentCalls,
		Favorites,
		Contacts
	}
}
