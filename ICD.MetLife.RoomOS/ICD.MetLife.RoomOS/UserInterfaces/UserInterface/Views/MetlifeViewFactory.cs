using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Camera;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Common;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial.DialNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Layout;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.MainNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking.Keyboard;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Share;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Volume;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.ShareMenu;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TouchTones;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner.NavSource;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Camera;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Common;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial.DialNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Layout;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.MainNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Blocking.Keyboard;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Lights;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Meetings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Share;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Volume;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.ShareMenu;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TouchTones;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TvTuner;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TvTuner.NavSource;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views
{
	/// <summary>
	/// Provides a way for presenters to access their views.
	/// </summary>
	public sealed class MetlifeViewFactory : IViewFactory
	{
		private delegate IView FactoryMethod(ISigInputOutput panel);

		private delegate IView ComponentFactoryMethod(ISigInputOutput panel, IVtProParent parent, ushort index);

		private readonly Dictionary<Type, ComponentFactoryMethod> m_ComponentViewFactories = new Dictionary
			<Type, ComponentFactoryMethod>
		{
			// Common
			{typeof(IDialNavComponentView), (panel, list, index) => new DialNavComponentView(panel, list, index)},
			{typeof(IMainNavComponentView), (panel, list, index) => new MainNavComponentView(panel, list, index)},
			{typeof(ICallStatusView), (panel, list, index) => new CallStatusView(panel, list, index)},

			// Dial
			{
				typeof(IFavoritesAndDirectoryComponentView),
				(panel, list, index) => new FavoritesAndDirectoryComponentView(panel, list, index)
			},
			{typeof(IRecentCallView), (panel, list, index) => new RecentCallView(panel, list, index)},

			// Lights
			{typeof(ILightComponentView), (panel, list, index) => new LightComponentView(panel, list, index)},
			{typeof(IShadeComponentView), (panel, list, index) => new ShadeComponentView(panel, list, index)},

			// Volume
			{typeof(IVolumeComponentView), (panel, list, index) => new VolumeComponentView(panel, list, index)},

			// Settings
			{
				typeof(ISettingsDeviceListComponentView),
				(panel, list, index) => new SettingsDeviceListComponentView(panel, list, index)
			},
			{
				typeof(ISettingsDevicePropertiesComponentView),
				(panel, list, index) => new SettingsDevicePropertiesComponentView(panel, list, index)
			},
			{
				typeof(ISettingsEventLogComponentView),
				(panel, list, index) => new SettingsEventLogComponentView(panel, list, index)
			},

			// ShareMenu
			{typeof(IShareComponentView), (panel, list, index) => new ShareComponentView(panel, list, index)},

			// TvTuner
			{typeof(IChannelPresetView), (panel, list, index) => new ChannelPresetView(panel, list, index)},
		};

		private readonly Dictionary<Type, FactoryMethod> m_ViewFactories = new Dictionary<Type, FactoryMethod>
		{
			// Misc
			{typeof(IHardButtonsView), panel => new HardButtonsView(panel)},
			{typeof(ISoundsView), panel => new SoundsView(panel)},

			// Common
			{typeof(IDialNavHomeButtonView), panel => new DialNavHomeButtonView(panel)},
			{typeof(IMainNavView), panel => new MainNavView(panel)},
			{typeof(IPageMainView), panel => new PageMainView(panel)},
			{typeof(IStatusBarView), panel => new StatusBarView(panel)},
			{typeof(IShutdownConfirmView), panel => new ShutdownConfirmView(panel)},

			// Dial
			{typeof(IDialNavView), panel => new DialNavView(panel)},
			{typeof(IDialpadView), panel => new DialpadView(panel)},
			{typeof(IFavoritesView), panel => new FavoritesView(panel)},
			{typeof(IDirectoryView), panel => new DirectoryView(panel)},
			{typeof(IKeyboardCommonView), panel => new KeyboardCommonView(panel)},
			{typeof(IKeyboardAlphaView), panel => new KeyboardAlphaView(panel)},
			{typeof(IKeyboardSpecialView), panel => new KeyboardSpecialView(panel)},
			{typeof(IRecentsView), panel => new RecentsView(panel)},

			// Home
			{typeof(IRoomNumbersView), panel => new RoomNumbersView(panel)},
			{typeof(IInACallView), panel => new InACallView(panel)},
			{typeof(IHomeMeetingsView), panel => new HomeMeetingsView(panel)},

			// Lights
			{typeof(ILightsView), panel => new LightsView(panel)},

			// Settings
			{typeof(ISettingsBaseView), panel => new SettingsBaseView(panel)},
			{typeof(ISettingsDeviceListView), panel => new SettingsDeviceListView(panel)},
			{typeof(ISettingsDevicePropertiesView), panel => new SettingsDevicePropertiesView(panel)},
			{typeof(ISettingsDeviceStatusView), panel => new SettingsDeviceStatusView(panel)},
			{typeof(ISettingsEventLogView), panel => new SettingsEventLogView(panel)},
			{typeof(ISettingsFileOperationsView), panel => new SettingsFileOperationsView(panel)},
			{typeof(ISettingsRoomInfoView), panel => new SettingsRoomInfoView(panel)},
			{typeof(ISettingsSystemConfigurationView), panel => new SettingsSystemConfigurationView(panel)},

			// ShareMenu
			{typeof(IShareMenuView), panel => new ShareMenuView(panel)},

			// TvTuner
			{typeof(ITvTunerView), panel => new TvTunerView(panel)},
			{typeof(IGenericNavSourceView), panel => new GenericNavSourceView(panel)},

			// Camera
			{typeof(ICameraView), panel => new CameraView(panel)},

			// Layout
			{typeof(ILayoutView), panel => new LayoutView(panel)},

			// TouchTones
			{typeof(ITouchTonesView), panel => new TouchTonesView(panel)},

			// Meetings
			{typeof(IMeetingsView), panel => new MeetingsView(panel)},

			// Popups
			{typeof(IVolumeSideView), panel => new VolumeSideView(panel)},
			{typeof(IVolumeView), panel => new VolumeView(panel)},
			{typeof(IPopupBaseView), panel => new PopupBaseView(panel)},
			{typeof(IPopupListView), panel => new PopupListView(panel)},
			{typeof(IIncomingCallView), panel => new IncomingCallView(panel)},
			{typeof(ISettingsStandardView), panel => new SettingsStandardView(panel)},
			{typeof(IPopupKeyboardCommonView), panel => new PopupKeyboardCommonView(panel)},
			{typeof(IPopupKeyboardAlphaView), panel => new PopupKeyboardAlphaView(panel)},
			{typeof(IPopupKeyboardSpecialView), panel => new PopupKeyboardSpecialView(panel)},
			{typeof(IAlertBoxView), panel => new AlertBoxView(panel)},
			{typeof(IDialogBoxView), panel => new DialogBoxView(panel)},
			{typeof(IReserveNowView), panel => new ReserveNowView(panel)},
			{typeof(IDisplaySelectView), panel => new DisplaySelectView(panel)},
		};

		private readonly IPanelDevice m_Panel;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public MetlifeViewFactory(IPanelDevice panel)
		{
			m_Panel = panel;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Instantiates a new view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetNewView<T>()
			where T : class, IView
		{
			if (!m_ViewFactories.ContainsKey(typeof(T)))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, typeof(T).Name);
				throw new KeyNotFoundException(message);
			}

			FactoryMethod factory = m_ViewFactories[typeof(T)];
			IView output = factory(m_Panel);

			if (output as T == null)
				throw new Exception(string.Format("View {0} is not of type {1}", output, typeof(T).Name));

			return output as T;
		}

		/// <summary>
		/// Instantiates a view of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private T GetNewView<T>(ISigInputOutput panel, IVtProParent parent, ushort index)
			where T : class, IView
		{
			if (!m_ComponentViewFactories.ContainsKey(typeof(T)))
			{
				string message = string.Format("{0} does not contain factory for {1}", GetType().Name, typeof(T).Name);
				throw new KeyNotFoundException(message);
			}

			ComponentFactoryMethod factory = m_ComponentViewFactories[typeof(T)];
			IView output = factory(panel, parent, index);

			if (output as T == null)
				throw new Exception(string.Format("View {0} is not of type {1}", output, typeof(T).Name));

			return output as T;
		}

		/// <summary>
		/// Creates views for the given subpage reference list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="childViews"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<T> GetNewSrlViews<T>(VtProSubpageReferenceList list, List<T> childViews, ushort count)
			where T : class, IView
		{
			count = Math.Min(count, list.MaxSize);

			for (ushort index = 0; index < list.MaxSize; index++)
				list.SetItemVisible(index, index < count);
			list.SetNumberOfItems(count);

			ISmartObject smartObject = list.SmartObject;

			for (ushort index = (ushort)childViews.Count; index < count; index++)
			{
				T view = GetNewView<T>(smartObject, list, index);
				childViews.Add(view);
			}

			return childViews.Take(count);
		}

		#endregion
	}
}
