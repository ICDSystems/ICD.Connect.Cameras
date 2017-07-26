using System;
using ICD.Connect.Settings.Core;
using ICD.Connect.Panels;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Camera;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Common;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial.DialNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Layout;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Lights;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Share;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.ShareMenu;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TouchTones;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner.NavSource;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.VisibilityTree;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface
{
	/// <summary>
	/// Holds the presenter/view hierarchy for a complete panel UI.
	/// </summary>
	public sealed class MetlifeUserInterface : IDisposable
	{
		private readonly MetlifeRoom m_Room;
		private readonly IPanelDevice m_Panel;
		private readonly INavigationController m_NavigationController;

		private SingleVisibilityNode m_NavBars;
		private SingleVisibilityNode m_Menus;
		private SingleVisibilityNode m_SettingsMenus;
		private DefaultVisibilityNode m_HomeVisibility;
		private SingleVisibilityNode m_PopupMenus;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="panel"></param>
		/// <param name="core"></param>
		public MetlifeUserInterface(MetlifeRoom room, IPanelDevice panel, ICore core)
		{
			m_Room = room;
			m_Panel = panel;

			IViewFactory viewFactory = new MetlifeViewFactory(panel);
			m_NavigationController = new MetlifeNavigationController(room, viewFactory, core);

			BuildVisibilityTree();

			Subscribe(m_Panel);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			Unsubscribe(m_Panel);

			m_NavigationController.Dispose();
		}

		/// <summary>
		/// Builds the rules for view visibility, e.g. prevent certain items from being visible at the same time.
		/// </summary>
		private void BuildVisibilityTree()
		{
			m_NavBars = new SingleVisibilityNode();
			m_NavBars.AddPresenter(m_NavigationController.LazyLoadPresenter<IMainNavPresenter>());
			m_NavBars.AddPresenter(m_NavigationController.LazyLoadPresenter<IDialNavPresenter>());

			SingleVisibilityNode navMenus = new SingleVisibilityNode();
			navMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<IShareMenuPresenter>());
			navMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ITvTunerPresenter>());
			navMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ICameraPresenter>());
			navMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ILayoutPresenter>());
			navMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ITouchTonesPresenter>());
			navMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<IMeetingsPresenter>());
			navMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<IGenericNavSourcePresenter>());

			SingleVisibilityNode dialMenus = new SingleVisibilityNode();
			dialMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<IDialpadPresenter>());
			dialMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<IDirectoryPresenter>());
			dialMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<IFavoritesPresenter>());
			dialMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<IKeyboardCommonPresenter>());
			dialMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<IRecentsPresenter>());

			m_PopupMenus = new SingleVisibilityNode();
			m_PopupMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsBasePresenter>());
			m_PopupMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ILightsPresenter>());
			m_PopupMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<IReserveNowPresenter>());
			m_PopupMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<IDisplaySelectPresenter>());

			m_SettingsMenus = new SingleVisibilityNode();
			m_SettingsMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsEventLogPresenter>());
			m_SettingsMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsFileOperationsPresenter>());
			m_SettingsMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsRoomInfoPresenter>());
			m_SettingsMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsSystemConfigurationPresenter>());
			m_SettingsMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsDeviceListPresenter>());
			m_SettingsMenus.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsDevicePropertiesPresenter>());

			m_Menus = new SingleVisibilityNode();
			m_Menus.AddNode(navMenus);
			m_Menus.AddNode(dialMenus);
			m_Menus.AddPresenter(m_NavigationController.LazyLoadPresenter<IInACallPresenter>());

			// The home page should always be visible when there is no nav/dial subpage on screen.
			m_HomeVisibility = new DefaultVisibilityNode(m_NavigationController.LazyLoadPresenter<IInACallPresenter>());
			m_HomeVisibility.AddNode(navMenus);
			m_HomeVisibility.AddNode(dialMenus);

			// These presenters are initially visible.
			m_NavigationController.NavigateTo<IMainNavPresenter>();
			m_NavigationController.NavigateTo<IStatusBarPresenter>();
			m_NavigationController.NavigateTo<IInACallPresenter>();
			m_NavigationController.NavigateTo<IHardButtonsPresenter>();
			m_NavigationController.NavigateTo<ISoundsPresenter>();

			// These presenters need to be instantiated as they control their own visibility
			m_NavigationController.LazyLoadPresenter<IShutdownConfirmPresenter>();
			m_NavigationController.LazyLoadPresenter<IIncomingCallPresenter>();
		}

		#region Panel Callbacks

		/// <summary>
		/// Subscribe to the panel events.
		/// </summary>
		/// <param name="panel"></param>
		private void Subscribe(IPanelDevice panel)
		{
			panel.OnAnyOutput += PanelOnAnyOutput;
		}

		/// <summary>
		/// Unsubscribes from the panel events.
		/// </summary>
		/// <param name="panel"></param>
		private void Unsubscribe(IPanelDevice panel)
		{
			panel.OnAnyOutput -= PanelOnAnyOutput;
		}

		/// <summary>
		/// Called when the user interacts with a panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void PanelOnAnyOutput(object sender, EventArgs eventArgs)
		{
			// User is using the panel.
			m_Room.IsOccupied = true;
			m_Room.ResetInactivityTimer();
		}

		#endregion
	}
}
