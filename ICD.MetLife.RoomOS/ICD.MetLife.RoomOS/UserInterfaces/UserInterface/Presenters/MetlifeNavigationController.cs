using System;
using System.Collections.Generic;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Camera;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Common;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial.DialNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial.DialNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Layout;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking.Keyboard;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Lights;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Share;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Volume;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings.SettingsDevicePropertiesComponents;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.ShareMenu;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TouchTones;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner.NavSource;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Camera;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Common;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial.DialNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial.DialNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Layout;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.MainNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.MainNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Blocking.Keyboard;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Lights;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Share;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Volume;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings.SettingsDevicePropertiesComponents;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.ShareMenu;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.TouchTones;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.TvTuner;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.TvTuner.NavSource;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters
{
	/// <summary>
	/// Provides a way for presenters to access each other.
	/// </summary>
	public sealed class MetlifeNavigationController : INavigationController
	{
		private delegate IPresenter PresenterFactory(
			int roomId, INavigationController nav, IViewFactory views, ICore core);

		private readonly Dictionary<Type, PresenterFactory> m_PresenterFactories = new Dictionary<Type, PresenterFactory>
		{
			// Misc
			{typeof(IHardButtonsPresenter), (roomId, nav, views, core) => new HardButtonsPresenter(roomId, nav, views, core)},
			{typeof(ISoundsPresenter), (roomId, nav, views, core) => new SoundsPresenter(roomId, nav, views, core)},

			// Common
			{typeof(IMainNavPresenter), (roomId, nav, views, core) => new MainNavPresenter(roomId, nav, views, core)},
			{typeof(IDialNavPresenter), (roomId, nav, views, core) => new DialNavPresenter(roomId, nav, views, core)},
			{
				typeof(IDialNavHomeButtonPresenter),
				(roomId, nav, views, core) => new DialNavHomeButtonPresenter(roomId, nav, views, core)
			},
			{typeof(ICallStatusPresenter), (roomId, nav, views, core) => new CallStatusPresenter(roomId, nav, views, core)},
			{typeof(IPageMainPresenter), (roomId, nav, views, core) => new PageMainPresenter(roomId, nav, views, core)},
			{typeof(IStatusBarPresenter), (roomId, nav, views, core) => new StatusBarPresenter(roomId, nav, views, core)},
			{
				typeof(IShutdownConfirmPresenter),
				(roomId, nav, views, core) => new ShutdownConfirmPresenter(roomId, nav, views, core)
			},

			// Main nav components
			{
				typeof(IMainNavAddCallComponentPresenter),
				(roomId, nav, views, core) => new MainNavAddCallComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavCameraComponentPresenter),
				(roomId, nav, views, core) => new MainNavCameraComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavContactsComponentPresenter),
				(roomId, nav, views, core) => new MainNavContactsComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavDialComponentPresenter),
				(roomId, nav, views, core) => new MainNavDialComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavEndCallComponentPresenter),
				(roomId, nav, views, core) => new MainNavEndCallComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavHoldCallComponentPresenter),
				(roomId, nav, views, core) => new MainNavHoldCallComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavMeetingsComponentPresenter),
				(roomId, nav, views, core) => new MainNavMeetingsComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavMuteComponentPresenter),
				(roomId, nav, views, core) => new MainNavMuteComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavPlayGameComponentPresenter),
				(roomId, nav, views, core) => new MainNavPlayGameComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavShareComponentPresenter),
				(roomId, nav, views, core) => new MainNavShareComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavTouchtonesComponentPresenter),
				(roomId, nav, views, core) => new MainNavTouchtonesComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavLayoutComponentPresenter),
				(roomId, nav, views, core) => new MainNavLayoutComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IMainNavSourceComponentPresenter),
				(roomId, nav, views, core) => new MainNavSourceComponentPresenter(roomId, nav, views, core)
			},

			// Dial nav components
			{
				typeof(IDialNavContactsComponentPresenter),
				(roomId, nav, views, core) => new DialNavContactsComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IDialNavDialpadComponentPresenter),
				(roomId, nav, views, core) => new DialNavDialpadComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IDialNavFavoritesComponentPresenter),
				(roomId, nav, views, core) => new DialNavFavoritesComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IDialNavKeyboardComponentPresenter),
				(roomId, nav, views, core) => new DialNavKeyboardComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IDialNavRecentCallsComponentPresenter),
				(roomId, nav, views, core) => new DialNavRecentCallsComponentPresenter(roomId, nav, views, core)
			},
			
			// Dial
			{typeof(IDialpadPresenter), (roomId, nav, views, core) => new DialpadPresenter(roomId, nav, views, core)},
			{
				typeof(IDirectoryContactComponentPresenter),
				(roomId, nav, views, core) => new DirectoryContactComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IDirectoryFolderComponentPresenter),
				(roomId, nav, views, core) => new DirectoryFolderComponentPresenter(roomId, nav, views, core)
			},
			{typeof(IFavoritesPresenter), (roomId, nav, views, core) => new FavoritesPresenter(roomId, nav, views, core)},
			{
				typeof(IFavoritesComponentPresenter),
				(roomId, nav, views, core) => new FavoritesComponentPresenter(roomId, nav, views, core)
			},
			{typeof(IDirectoryPresenter), (roomId, nav, views, core) => new DirectoryPresenter(roomId, nav, views, core)},
			{typeof(IKeyboardAlphaPresenter), (roomId, nav, views, core) => new KeyboardAlphaPresenter(roomId, nav, views, core)},
			{
				typeof(IKeyboardCommonPresenter),
				(roomId, nav, views, core) => new KeyboardCommonPresenter(roomId, nav, views, core)
			},
			{
				typeof(IKeyboardSpecialPresenter),
				(roomId, nav, views, core) => new KeyboardSpecialPresenter(roomId, nav, views, core)
			},
			{typeof(IRecentCallPresenter), (roomId, nav, views, core) => new RecentCallPresenter(roomId, nav, views, core)},
			{typeof(IRecentsPresenter), (roomId, nav, views, core) => new RecentsPresenter(roomId, nav, views, core)},

			// Home
			{typeof(IRoomNumbersPresenter), (roomId, nav, views, core) => new RoomNumbersPresenter(roomId, nav, views, core)},
			{typeof(IInACallPresenter), (roomId, nav, views, core) => new InACallPresenter(roomId, nav, views, core)},
			{typeof(IHomeMeetingsPresenter), (roomId, nav, views, core) => new HomeMeetingsPresenter(roomId, nav, views, core)},

			// ShareMenu
			{typeof(IShareMenuPresenter), (roomId, nav, views, core) => new ShareMenuPresenter(roomId, nav, views, core)},
			{
				typeof(IShareComponentPresenter),
				(roomId, nav, views, core) => new ShareComponentPresenter(roomId, nav, views, core)
			},

			// Settings
			{typeof(ISettingsBasePresenter), (roomId, nav, views, core) => new SettingsBasePresenter(roomId, nav, views, core)},
			{
				typeof(ISettingsFileOperationsPresenter),
				(roomId, nav, views, core) => new SettingsFileOperationsPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsDeviceListComponentPresenter),
				(roomId, nav, views, core) => new SettingsDeviceListComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsDeviceListPresenter),
				(roomId, nav, views, core) => new SettingsDeviceListPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsDevicePropertiesDeviceComponentPresenter),
				(roomId, nav, views, core) => new SettingsDevicePropertiesDeviceComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsDevicePropertiesEnumComponentPresenter),
				(roomId, nav, views, core) => new SettingsDevicePropertiesEnumComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsDevicePropertiesIpidComponentPresenter),
				(roomId, nav, views, core) => new SettingsDevicePropertiesIpidComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsDevicePropertiesPortComponentPresenter),
				(roomId, nav, views, core) => new SettingsDevicePropertiesPortComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsDevicePropertiesStringComponentPresenter),
				(roomId, nav, views, core) => new SettingsDevicePropertiesStringComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsDevicePropertiesPresenter),
				(roomId, nav, views, core) => new SettingsDevicePropertiesPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsEventLogPresenter),
				(roomId, nav, views, core) => new SettingsEventLogPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsEventLogComponentPresenter),
				(roomId, nav, views, core) => new SettingsEventLogComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsRoomInfoPresenter),
				(roomId, nav, views, core) => new SettingsRoomInfoPresenter(roomId, nav, views, core)
			},
			{
				typeof(ISettingsSystemConfigurationPresenter),
				(roomId, nav, views, core) => new SettingsSystemConfigurationPresenter(roomId, nav, views, core)
			},

			// TvTuner
			{typeof(IChannelPresetPresenter), (roomId, nav, views, core) => new ChannelPresetPresenter(roomId, nav, views, core)},
			{typeof(ITvTunerPresenter), (roomId, nav, views, core) => new TvTunerPresenter(roomId, nav, views, core)},
			{
				typeof(IGenericNavSourcePresenter),
				(roomId, nav, views, core) => new GenericNavSourcePresenter(roomId, nav, views, core)
			},

			// Camera
			{typeof(ICameraPresenter), (roomId, nav, views, core) => new CameraPresenter(roomId, nav, views, core)},

			// Layout
			{typeof(ILayoutPresenter), (roomId, nav, views, core) => new LayoutPresenter(roomId, nav, views, core)},

			// Meetings
			{typeof(IMeetingsPresenter), (roomId, nav, views, core) => new MeetingsPresenter(roomId, nav, views, core)},


			// TouchTones
			{typeof(ITouchTonesPresenter), (roomId, nav, views, core) => new TouchTonesPresenter(roomId, nav, views, core)},

			// Popups
			{typeof(IVolumeSidePresenter), (roomId, nav, views, core) => new VolumeSidePresenter(roomId, nav, views, core)},
			{typeof(IVolumePresenter), (roomId, nav, views, core) => new VolumePresenter(roomId, nav, views, core)},
			{
				typeof(IVolumeComponentPresenter),
				(roomId, nav, views, core) => new VolumeComponentPresenter(roomId, nav, views, core)
			},
			{typeof(ILightsPresenter), (roomId, nav, views, core) => new LightsPresenter(roomId, nav, views, core)},
			{
				typeof(ILightComponentPresenter),
				(roomId, nav, views, core) => new LightComponentPresenter(roomId, nav, views, core)
			},
			{
				typeof(IShadeComponentPresenter),
				(roomId, nav, views, core) => new ShadeComponentPresenter(roomId, nav, views, core)
			},
			{typeof(IPopupBasePresenter), (roomId, nav, views, core) => new PopupBasePresenter(roomId, nav, views, core)},
			{typeof(IIncomingCallPresenter), (roomId, nav, views, core) => new IncomingCallPresenter(roomId, nav, views, core)},
			{
				typeof(ISettingsStandardPresenter),
				(roomId, nav, views, core) => new SettingsStandardPresenter(roomId, nav, views, core)
			},
			{
				typeof(IPopupKeyboardCommonPresenter),
				(roomId, nav, views, core) => new PopupKeyboardCommonPresenter(roomId, nav, views, core)
			},
			{
				typeof(IPopupKeyboardAlphaPresenter),
				(roomId, nav, views, core) => new PopupKeyboardAlphaPresenter(roomId, nav, views, core)
			},
			{
				typeof(IPopupKeyboardSpecialPresenter),
				(roomId, nav, views, core) => new PopupKeyboardSpecialPresenter(roomId, nav, views, core)
			},
			{typeof(IPopupListPresenter), (roomId, nav, views, core) => new PopupListPresenter(roomId, nav, views, core)},
			{typeof(IDialogBoxPresenter), (roomId, nav, views, core) => new DialogBoxPresenter(roomId, nav, views, core)},
			{typeof(IAlertBoxPresenter), (roomId, nav, views, core) => new AlertBoxPresenter(roomId, nav, views, core)},
			{typeof(IReserveNowPresenter), (roomId, nav, views, core) => new ReserveNowPresenter(roomId, nav, views, core)},
			{typeof(IDisplaySelectPresenter), (roomId, nav, views, core) => new DisplaySelectPresenter(roomId, nav, views, core)},
		};

		private readonly Dictionary<Type, IPresenter> m_Cache;
		private readonly SafeCriticalSection m_CacheSection;
		private readonly MetlifeRoom m_Room;
		private readonly IViewFactory m_ViewFactory;
		private readonly ICore m_Core;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="viewFactory"></param>
		/// <param name="core"></param>
		public MetlifeNavigationController(MetlifeRoom room, IViewFactory viewFactory, ICore core)
		{
			m_Cache = new Dictionary<Type, IPresenter>();
			m_CacheSection = new SafeCriticalSection();

			m_Room = room;
			m_ViewFactory = viewFactory;
			m_Core = core;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_Cache.Values.ForEach(p => p.Dispose());
			m_Cache.Clear();
		}

		/// <summary>
		/// Instantiates or returns an existing presenter of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IPresenter LazyLoadPresenter(Type type)
		{
			IPresenter output;

			m_CacheSection.Enter();

			try
			{
				if (!m_Cache.ContainsKey(type))
					m_Cache[type] = GetNewPresenter(type);
				output = m_Cache[type];
			}
			finally
			{
				m_CacheSection.Leave();
			}

			return output;
		}

		/// <summary>
		/// Instantiates a new presenter of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IPresenter GetNewPresenter(Type type)
		{
			if (!m_PresenterFactories.ContainsKey(type))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, type.Name);
				throw new KeyNotFoundException(message);
			}

			PresenterFactory factory = m_PresenterFactories[type];
			IPresenter output = factory(m_Room.Id, this, m_ViewFactory, m_Core);

			if (!type.IsInstanceOfType(output))
				throw new Exception(string.Format("Presenter {0} is not of type {1}", output, type.Name));

			return output;
		}

		#endregion
	}
}
