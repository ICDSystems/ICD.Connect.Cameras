using ICD.Common.Attributes.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Connect.Settings;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings.SettingsDevicePropertiesComponents;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsDevicePropertiesPresenter : AbstractPresenter<ISettingsDevicePropertiesView>,
	                                                        ISettingsDevicePropertiesPresenter
	{
		private readonly SettingsDevicePropertiesComponentPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		private ISettings m_Settings;

		#region Properties

		/// <summary>
		/// Gets/sets the current settings instance.
		/// </summary>
		public ISettings Settings
		{
			get { return m_Settings; }
			set
			{
				if (value == m_Settings)
					return;

				m_Settings = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets the title for the component.
		/// </summary>
		private string Title
		{
			get
			{
				return m_Settings == null
					       ? string.Empty
					       : string.Format("{0} - {1} ({2})", m_Settings.Id, m_Settings.Name, m_Settings.FactoryName);
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
		public SettingsDevicePropertiesPresenter(int room, INavigationController nav, IViewFactory views,
		                                         ICore core)
			: base(room, nav, views, core)
		{
			m_ChildrenFactory = new SettingsDevicePropertiesComponentPresenterFactory(nav, ItemFactory);
			m_RefreshSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsDevicePropertiesView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<PropertySettingsPair> pairs = GetProperties();
				foreach (ISettingsDevicePropertiesComponentPresenter presenter in m_ChildrenFactory.BuildChildren(pairs))
					presenter.ShowView(true);

				view.SetTitle(Title);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Generates the given number of views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<ISettingsDevicePropertiesComponentView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		/// <summary>
		/// Gets the properties for the current settings.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		private IEnumerable<PropertySettingsPair> GetProperties()
		{
			if (m_Settings == null)
				throw new InvalidOperationException("Settings is null");

			CType type = m_Settings.GetType();

			return type.GetProperties()
			           .Where(IsValidProperty)
			           .Select(p => new PropertySettingsPair(p, m_Settings));
		}

		/// <summary>
		/// Returns true if we're interested in showing the property in the UI.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		private static bool IsValidProperty(PropertyInfo property)
		{
			if (!property.CanRead || !property.CanWrite)
				return false;

			if (SettingsDevicePropertiesComponentPresenterFactory.GetPropertyType(property) ==
			    SettingsProperty.ePropertyType.Hidden)
				return false;

			return !property.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsDevicePropertiesView view)
		{
			base.Subscribe(view);

			view.OnExitButtonPressed += ViewOnExitButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsDevicePropertiesView view)
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
			Navigation.NavigateTo<ISettingsDeviceListPresenter>();
		}

		#endregion
	}
}
