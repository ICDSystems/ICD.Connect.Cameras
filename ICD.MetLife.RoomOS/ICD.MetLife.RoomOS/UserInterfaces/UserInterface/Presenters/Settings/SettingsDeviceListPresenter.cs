using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Devices;
using ICD.Connect.Krang.Settings;
using ICD.Connect.Panels;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.Endpoints.Destinations;
using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsDeviceListPresenter : AbstractPresenter<ISettingsDeviceListView>,
	                                                  ISettingsDeviceListPresenter
	{
		private readonly SettingsDeviceListComponentPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		private eSettingsType m_Mode;

		private IPopupListPresenter m_PopupList;
		private IDialogBoxPresenter m_PopupConfirmation;

		#region Properties

		/// <summary>
		/// Gets/sets the current settings type.
		/// </summary>
		public eSettingsType Mode
		{
			get { return m_Mode; }
			set
			{
				if (value == m_Mode)
					return;

				m_Mode = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets the settings collection for the current settings type.
		/// </summary>
		private SettingsCollection Collection
		{
			get
			{
				ICoreSettings settings = Navigation.LazyLoadPresenter<ISettingsBasePresenter>().SettingsInstance;
				return settings.OriginatorSettings;
			}
		}

		/// <summary>
		/// Gets the room settings collection containing the ids of the current collection.
		/// </summary>
		private IcdHashSet<int> RoomDependenciesCollection { get { return GetRoomDependenciesCollection(Mode); } }

		/// <summary>
		/// Gets the popup list menu.
		/// </summary>
		private IPopupListPresenter PopupList
		{
			get { return m_PopupList ?? (m_PopupList = Navigation.LazyLoadPresenter<IPopupListPresenter>()); }
		}

		/// <summary>
		/// Gets the popup confirmation menu.
		/// </summary>
		private IDialogBoxPresenter PopupConfirmation
		{
			get { return m_PopupConfirmation ?? (m_PopupConfirmation = Navigation.LazyLoadPresenter<IDialogBoxPresenter>()); }
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsDeviceListPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_ChildrenFactory = new SettingsDeviceListComponentPresenterFactory(nav, ItemFactory);
			m_RefreshSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			UnsubscribeChildren();
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsDeviceListView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				UnsubscribeChildren();

				// Append null for the "new" item.
				ISettings[] settings = GetSettingsForCurrentMode().Append(null).ToArray();

				foreach (ISettingsDeviceListComponentPresenter presenter in m_ChildrenFactory.BuildChildren(settings))
				{
					Subscribe(presenter);
					presenter.ShowView(true);
				}

				view.SetTitle(StringUtils.NiceName(m_Mode));
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private IEnumerable<ISettings> GetSettingsForCurrentMode()
		{
			Type originatorType;

			switch (Mode)
			{
				case eSettingsType.Devices:
					originatorType = typeof(IDevice);
					break;
				case eSettingsType.Ports:
					originatorType = typeof(IPort);
					break;
				case eSettingsType.Panels:
					originatorType = typeof(IPanelDevice);
					break;
				case eSettingsType.Connections:
					originatorType = typeof(Connection);
					break;
				case eSettingsType.Sources:
					originatorType = typeof(ISource);
					break;
				case eSettingsType.Destinations:
					originatorType = typeof(IDestination);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return Collection.Where(s => s.OriginatorType.IsAssignableTo(originatorType));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Unsubscribes from all of the child events.
		/// </summary>
		private void UnsubscribeChildren()
		{
			foreach (ISettingsDeviceListComponentPresenter presenter in m_ChildrenFactory)
				Unsubscribe(presenter);
		}

		/// <summary>
		/// Generates the given number of child views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<ISettingsDeviceListComponentView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		/// <summary>
		/// Show the popup to create a new device settings instance.
		/// </summary>
		private void ShowNewPopup()
		{
			string singular = GetModeSingular();
			string title = string.Format("Create new {0}", singular);
			string[] items = GetNewItemList().ToArray();

			// If there is only one item select it by default
			if (items.Length == 1)
			{
				NewItemSelected(items[0]);
				return;
			}

			PopupList.SetListItems(items, GetListItemLabel, title, NewItemSelected);
			PopupList.ShowView(true);
		}

		/// <summary>
		/// Gets the label for each item in the popup list.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private static string GetListItemLabel(object item)
		{
			return item as string;
		}

		/// <summary>
		/// Called when the user selects a new device from the popup list.
		/// </summary>
		/// <param name="item"></param>
		private void NewItemSelected(object item)
		{
			PopupList.ShowView(false);

			string name = item as string;
			ISettings settings;

			switch (m_Mode)
			{
				case eSettingsType.Devices:
					settings = PluginFactory.InstantiateDefault<XmlDeviceSettingsFactoryMethodAttribute>(name);
					break;
				case eSettingsType.Ports:
					settings = PluginFactory.InstantiateDefault<XmlPortSettingsFactoryMethodAttribute>(name);
					break;
				case eSettingsType.Panels:
					settings = PluginFactory.InstantiateDefault<XmlPanelSettingsFactoryMethodAttribute>(name);
					break;
				case eSettingsType.Connections:
					settings = PluginFactory.InstantiateDefault<XmlConnectionSettingsFactoryMethodAttribute>(name);
					break;
				case eSettingsType.Sources:
					settings = PluginFactory.InstantiateDefault<XmlSourceSettingsFactoryMethodAttribute>(name);
					break;
				case eSettingsType.Destinations:
					settings = PluginFactory.InstantiateDefault<XmlDestinationSettingsFactoryMethodAttribute>(name);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			settings.Name = "Unnamed";
			settings.Id = Collection.GetNewId();

			AddSettings(settings);
		}

		/// <summary>
		/// Returns the list of device types for the 
		/// </summary>
		/// <returns></returns>
		private IEnumerable<string> GetNewItemList()
		{
			switch (m_Mode)
			{
				case eSettingsType.Devices:
					return PluginFactory.GetFactoryNames<XmlDeviceSettingsFactoryMethodAttribute>();
				case eSettingsType.Ports:
					return PluginFactory.GetFactoryNames<XmlPortSettingsFactoryMethodAttribute>();
				case eSettingsType.Panels:
					return PluginFactory.GetFactoryNames<XmlPanelSettingsFactoryMethodAttribute>();
				case eSettingsType.Connections:
					return PluginFactory.GetFactoryNames<XmlConnectionSettingsFactoryMethodAttribute>();
				case eSettingsType.Sources:
					return PluginFactory.GetFactoryNames<XmlSourceSettingsFactoryMethodAttribute>();
				case eSettingsType.Destinations:
					return PluginFactory.GetFactoryNames<XmlDestinationSettingsFactoryMethodAttribute>();

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Gets the room dependencies collection (e.g. the collection of device ids) based on the current mode.
		/// Returns null for Connections.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[CanBeNull]
		private IcdHashSet<int> GetRoomDependenciesCollection(eSettingsType type)
		{
			IRoomSettings roomSettings = Collection.OfType<IRoomSettings>().FirstOrDefault();

			if (roomSettings == null)
				throw new InvalidOperationException();

			switch (type)
			{
				case eSettingsType.Devices:
					return roomSettings.Devices;
				case eSettingsType.Ports:
					return roomSettings.Ports;
				case eSettingsType.Panels:
					return roomSettings.Panels;
				case eSettingsType.Connections:
					return null;
				case eSettingsType.Sources:
					return roomSettings.Sources;
				case eSettingsType.Destinations:
					return roomSettings.Destinations;
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}

		/// <summary>
		/// Simply returns the singular form of the mode string.
		/// </summary>
		/// <returns></returns>
		private string GetModeSingular()
		{
			switch (m_Mode)
			{
				case eSettingsType.Devices:
					return "Device";
				case eSettingsType.Ports:
					return "Port";
				case eSettingsType.Panels:
					return "Panel";
				case eSettingsType.Connections:
					return "Connection";
				case eSettingsType.Sources:
					return "Source";
				case eSettingsType.Destinations:
					return "Destination";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Adds the settings to the core and the room.
		/// </summary>
		/// <param name="settings"></param>
		private void AddSettings(ISettings settings)
		{
			Collection.Add(settings);

			if (RoomDependenciesCollection != null)
				RoomDependenciesCollection.Add(settings.Id);

			RefreshIfVisible();
		}

		/// <summary>
		/// Removes the settings instance from the core and the room.
		/// </summary>
		/// <param name="settings"></param>
		private void RemoveSettings(ISettings settings)
		{
			Collection.Remove(settings);

			if (RoomDependenciesCollection != null)
				RoomDependenciesCollection.Remove(settings.Id);

			RefreshIfVisible();
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Subscribe to the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Subscribe(ISettingsDeviceListComponentPresenter child)
		{
			child.OnDeleteButtonPressed += ChildOnDeleteButtonPressed;
			child.OnItemButtonPressed += ChildOnItemButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the subpage events.
		/// </summary>
		/// <param name="child"></param>
		private void Unsubscribe(ISettingsDeviceListComponentPresenter child)
		{
			child.OnDeleteButtonPressed -= ChildOnDeleteButtonPressed;
			child.OnItemButtonPressed -= ChildOnItemButtonPressed;
		}

		/// <summary>
		/// Called when a subpage item is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildOnItemButtonPressed(object sender, EventArgs eventArgs)
		{
			ISettingsDeviceListComponentPresenter child = sender as ISettingsDeviceListComponentPresenter;
			if (child == null)
				return;

			ISettings settings = child.Settings;

			// Add a new item.
			if (settings == null)
			{
				// Edge case - only one type of source, destination or connection
				switch (Mode)
				{
					case eSettingsType.Connections:
						NewItemSelected(PluginFactory.GetFactoryName<ConnectionSettings>());
						break;
					case eSettingsType.Sources:
						NewItemSelected(PluginFactory.GetFactoryName<MetlifeSourceSettings>());
						break;
					case eSettingsType.Destinations:
						NewItemSelected(PluginFactory.GetFactoryName<MetlifeDestinationSettings>());
						break;

					default:
						ShowNewPopup();
						break;
				}
				
				return;
			}

			Navigation.NavigateTo<ISettingsDevicePropertiesPresenter>().Settings = settings;
		}

		/// <summary>
		/// Called when a subpage delete button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildOnDeleteButtonPressed(object sender, EventArgs eventArgs)
		{
			ISettingsDeviceListComponentPresenter child = sender as ISettingsDeviceListComponentPresenter;
			if (child == null)
				return;

			ISettings settings = child.Settings;
			if (settings == null)
				return;

			string message = string.Format("Confirm deletion of {0}", child.Title);

			DialogBoxButton[] buttons =
			{
				new DialogBoxButton("OK", () => RemoveSettings(settings)),
				new DialogBoxButton("Cancel")
			};

			PopupConfirmation.SetDialog(message, buttons);
			PopupConfirmation.ShowView(true);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsDeviceListView view)
		{
			base.Subscribe(view);

			view.OnExitButtonPressed += ViewOnExitButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsDeviceListView view)
		{
			base.Unsubscribe(view);

			view.OnExitButtonPressed -= ViewOnExitButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the exit button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnExitButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.NavigateTo<ISettingsSystemConfigurationPresenter>();
		}

		#endregion
	}
}
