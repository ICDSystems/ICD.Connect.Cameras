using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsBasePresenter : AbstractPresenter<ISettingsBaseView>, ISettingsBasePresenter
	{
		private readonly Type[] m_MenuPages =
		{
			typeof(ISettingsRoomInfoPresenter),
			typeof(ISettingsSystemConfigurationPresenter),
			typeof(ISettingsEventLogPresenter),
			typeof(ISettingsFileOperationsPresenter)
		};

		private readonly Type[] m_SettingsPages =
		{
			typeof(ISettingsRoomInfoPresenter),
			typeof(ISettingsSystemConfigurationPresenter),
			typeof(ISettingsEventLogPresenter),
			typeof(ISettingsFileOperationsPresenter),
			typeof(ISettingsDeviceListPresenter),
			typeof(ISettingsDevicePropertiesPresenter)
		};

		private readonly Dictionary<Type, string> m_MenuNames = new Dictionary<Type, string>
		{
			{typeof(ISettingsRoomInfoPresenter), "Room Info"},
			{typeof(ISettingsSystemConfigurationPresenter), "System"},
			{typeof(ISettingsEventLogPresenter), "Event Log"},
			{typeof(ISettingsFileOperationsPresenter), "File Operations"},
		};

		private readonly SafeCriticalSection m_ChildVisibilitySection;

		/// <summary>
		/// We store the user configured settings in this property until the user wishes to save.
		/// </summary>
		public ICoreSettings SettingsInstance { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsBasePresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_ChildVisibilitySection = new SafeCriticalSection();

			SettingsInstance = core.CopySettings();
		}

		/// <summary>
		/// Updates the SettingsInstance property with the current state of the core.
		/// </summary>
		public void RevertSettingsInstance()
		{
			Core.CopySettings(SettingsInstance);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsBaseView view)
		{
			base.Refresh(view);

			view.SetButtonLabels(m_MenuPages.Select(t => m_MenuNames[t]));
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsBaseView view)
		{
			base.Subscribe(view);

			view.OnItemPressed += ViewOnItemPressed;
			view.OnExitButtonPressed += ViewOnExitButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsBaseView view)
		{
			base.Unsubscribe(view);

			view.OnItemPressed -= ViewOnItemPressed;
			view.OnExitButtonPressed -= ViewOnExitButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the exit button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnExitButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		/// <summary>
		/// Called when the user presses an item button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="uShortEventArgs"></param>
		private void ViewOnItemPressed(object sender, UShortEventArgs uShortEventArgs)
		{
			m_ChildVisibilitySection.Enter();

			try
			{
				Type type = m_MenuPages[uShortEventArgs.Data];
				Navigation.LazyLoadPresenter(type).ShowView(true);
			}
			finally
			{
				m_ChildVisibilitySection.Leave();
			}
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_ChildVisibilitySection.Enter();

			try
			{
				// Show the first item when the view becomes visible, otherwise hide everything.
				Type type = args.Data ? m_MenuPages.First() : null;
				foreach (Type menuType in m_SettingsPages)
					Navigation.LazyLoadPresenter(menuType).ShowView(menuType == type);
			}
			finally
			{
				m_ChildVisibilitySection.Leave();
			}
		}

		#endregion
	}
}
