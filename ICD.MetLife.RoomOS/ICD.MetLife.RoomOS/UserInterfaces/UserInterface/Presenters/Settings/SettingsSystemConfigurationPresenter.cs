using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsSystemConfigurationPresenter : AbstractPresenter<ISettingsSystemConfigurationView>,
	                                                           ISettingsSystemConfigurationPresenter
	{
		private const ushort INDEX_PANELS = 0;
		private const ushort INDEX_DEVICES = 1;
		private const ushort INDEX_PORTS = 2;
		private const ushort INDEX_ROUTING = 3;
		private const ushort INDEX_SOURCES = 4;
		private const ushort INDEX_DESTINATIONS = 5;

		private static readonly ushort[] s_MenuIndices =
		{
			INDEX_PANELS,
			INDEX_PORTS,
			INDEX_DEVICES,
			INDEX_ROUTING,
			INDEX_SOURCES,
			INDEX_DESTINATIONS
		};

		private static readonly Dictionary<ushort, string> s_Labels = new Dictionary<ushort, string>
		{
			{INDEX_PANELS, "Panels"},
			{INDEX_DEVICES, "Devices"},
			{INDEX_PORTS, "Ports"},
			{INDEX_ROUTING, "Connections"},
			{INDEX_SOURCES, "Sources"},
			{INDEX_DESTINATIONS, "Destinations"},
		};

		private ISettingsDeviceListPresenter m_SystemDeviceList;

		#region Properties

		/// <summary>
		/// Gets the Device Lise menu.
		/// </summary>
		private ISettingsDeviceListPresenter SystemDeviceList
		{
			get
			{
				return m_SystemDeviceList ?? (m_SystemDeviceList = Navigation.LazyLoadPresenter<ISettingsDeviceListPresenter>());
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsSystemConfigurationPresenter(int room, INavigationController nav,
		                                            IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsSystemConfigurationView view)
		{
			base.Refresh(view);

			string[] labels = s_MenuIndices.Select(i => s_Labels[i]).ToArray();
			view.SetButtonLabels(labels);
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsSystemConfigurationView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsSystemConfigurationView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnButtonPressed(object sender, UShortEventArgs args)
		{
			ushort menu = s_MenuIndices[args.Data];

			SystemDeviceList.Mode = GetSettingsType(menu);
			SystemDeviceList.ShowView(true);
		}

		private static eSettingsType GetSettingsType(ushort index)
		{
			switch (index)
			{
				case INDEX_DEVICES:
					return eSettingsType.Devices;

				case INDEX_PANELS:
					return eSettingsType.Panels;

				case INDEX_ROUTING:
					return eSettingsType.Connections;

				case INDEX_PORTS:
					return eSettingsType.Ports;

				case INDEX_SOURCES:
					return eSettingsType.Sources;

				case INDEX_DESTINATIONS:
					return eSettingsType.Destinations;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion
	}
}
