using System.Collections.Generic;
using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views
{
	public abstract class VtProIcon : IIcon
	{
		private const string SUFFIX_DEFAULT = "_01";
		private const string SUFFIX_ACTIVE = "_06";

		// Main nav icons
		protected const string MAIN_NAV_AUDIO = "mn_audio";
		protected const string MAIN_NAV_MUTE = "mn_mute";
		protected const string MAIN_NAV_HOLD_CALL = "mn_holdCall";
		protected const string MAIN_NAV_END_CALL = "mn_endCall";
		protected const string MAIN_NAV_SHARE = "mn_share";
		protected const string MAIN_NAV_CAMERA = "mn_camera";
		protected const string MAIN_NAV_ADD_CALL = "mn_addCall";
		protected const string MAIN_NAV_DIAL = "mn_dial";
		protected const string MAIN_NAV_CONTACTS = "mn_contacts";
		protected const string MAIN_NAV_SCHEDULER = "mn_scheduler";
		protected const string MAIN_NAV_LAYOUT = "mn_layout";
		protected const string MAIN_NAV_WATCH_TV = "mn_watchTV";
		protected const string MAIN_NAV_PLAY_GAME = "mn_playGame";
		protected const string MAIN_NAV_DIALPAD = DIAL_DIALPAD;

		// Share icons
		protected const string SHARE_CLICKSHARE = "share_clickshare";
		protected const string SHARE_FLOORBOX = "share_floorbox";
		protected const string SHARE_INROOM_PC = "share_inroomPC";
		protected const string SHARE_LAPTOP = "share_laptop";
		protected const string SHARE_PODIUM = "share_podium";
		protected const string SHARE_WALLPLATE = "share_wallplate";
		protected const string SHARE_CABLEBOX = MAIN_NAV_WATCH_TV;
		protected const string SHARE_GAME = MAIN_NAV_PLAY_GAME;
		protected const string SHARE_CAMERA = MAIN_NAV_CAMERA;

		// Dial icons
		protected const string DIAL_DIALPAD = "dial_dialpad";
		protected const string DIAL_KEYBOARD = "dial_keyboard";
		protected const string DIAL_RECENT_CALLS = "dial_recentCalls";
		protected const string DIAL_FAVORITES = "dial_favorites";
		protected const string DIAL_CONTACTS = "dial_contacts";

		private static readonly Dictionary<eIconState, string> s_IconStateSuffixes = new Dictionary<eIconState, string>
		{
			{eIconState.Default, SUFFIX_DEFAULT},
			{eIconState.Active, SUFFIX_ACTIVE}
		};

		private readonly string m_IconString;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="iconString"></param>
		protected VtProIcon(string iconString)
		{
			m_IconString = iconString;
		}

		/// <summary>
		/// Gets the icon string with the suffix for the given state.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public string GetIconString(eIconState state)
		{
			return m_IconString + s_IconStateSuffixes[state];
		}
	}

	public sealed class SourceIcon : VtProIcon, IMainNavIcon, ISourceIcon
	{
		private static readonly Dictionary<eSourceType, SourceIcon> s_SourceTypeIcons =
			new Dictionary<eSourceType, SourceIcon>
			{
				{eSourceType.Audio, new SourceIcon(MAIN_NAV_AUDIO)},
				{eSourceType.Wireless, new SourceIcon(SHARE_CLICKSHARE)},
				{eSourceType.Floorbox, new SourceIcon(SHARE_FLOORBOX)},
				{eSourceType.Pc, new SourceIcon(SHARE_INROOM_PC)},
				{eSourceType.Laptop, new SourceIcon(SHARE_LAPTOP)},
				{eSourceType.Podium, new SourceIcon(SHARE_PODIUM)},
				{eSourceType.Wallplate, new SourceIcon(SHARE_WALLPLATE)},
				{eSourceType.CableBox, new SourceIcon(SHARE_CABLEBOX)},
				{eSourceType.Game, new SourceIcon(SHARE_GAME)},
				{eSourceType.Camera, new SourceIcon(SHARE_CAMERA)}
			};

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="iconString"></param>
		private SourceIcon(string iconString)
			: base(iconString)
		{
		}

		public static SourceIcon FromSourceType(eSourceType sourceType)
		{
			return s_SourceTypeIcons[sourceType];
		}
	}

	public sealed class DialNavIcon : VtProIcon, IDialNavIcon
	{
		private static readonly Dictionary<eDialNavIcon, DialNavIcon> s_DialNavIcons =
			new Dictionary<eDialNavIcon, DialNavIcon>
			{
				{eDialNavIcon.Dialpad, new DialNavIcon(DIAL_DIALPAD)},
				{eDialNavIcon.Keyboard, new DialNavIcon(DIAL_KEYBOARD)},
				{eDialNavIcon.RecentCalls, new DialNavIcon(DIAL_RECENT_CALLS)},
				{eDialNavIcon.Favorites, new DialNavIcon(DIAL_FAVORITES)},
				{eDialNavIcon.Contacts, new DialNavIcon(DIAL_CONTACTS)}
			};

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="iconString"></param>
		private DialNavIcon(string iconString)
			: base(iconString)
		{
		}

		public static DialNavIcon FromDialNavType(eDialNavIcon dialNav)
		{
			return s_DialNavIcons[dialNav];
		}
	}

	public sealed class MainNavIcon : VtProIcon, IMainNavIcon
	{
		private static readonly Dictionary<eMainNavIcon, MainNavIcon> s_MainNavIcons =
			new Dictionary<eMainNavIcon, MainNavIcon>
			{
				{eMainNavIcon.PrivacyMute, new MainNavIcon(MAIN_NAV_MUTE)},
				{eMainNavIcon.HoldCall, new MainNavIcon(MAIN_NAV_HOLD_CALL)},
				{eMainNavIcon.EndCall, new MainNavIcon(MAIN_NAV_END_CALL)},
				{eMainNavIcon.Share, new MainNavIcon(MAIN_NAV_SHARE)},
				{eMainNavIcon.Camera, new MainNavIcon(MAIN_NAV_CAMERA)},
				{eMainNavIcon.Touchtones, new MainNavIcon(MAIN_NAV_DIALPAD)},
				{eMainNavIcon.AddCall, new MainNavIcon(MAIN_NAV_ADD_CALL)},
				{eMainNavIcon.Dial, new MainNavIcon(MAIN_NAV_DIAL)},
				{eMainNavIcon.Contacts, new MainNavIcon(MAIN_NAV_CONTACTS)},
				{eMainNavIcon.Meetings, new MainNavIcon(MAIN_NAV_SCHEDULER)},
				{eMainNavIcon.Layout, new MainNavIcon(MAIN_NAV_LAYOUT)},
				{eMainNavIcon.WatchTv, new MainNavIcon(MAIN_NAV_WATCH_TV)},
				{eMainNavIcon.PlayGame, new MainNavIcon(MAIN_NAV_PLAY_GAME)}
			};

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="iconString"></param>
		private MainNavIcon(string iconString)
			: base(iconString)
		{
		}

		public static MainNavIcon FromMainNavType(eMainNavIcon mainNav)
		{
			return s_MainNavIcons[mainNav];
		}
	}
}
