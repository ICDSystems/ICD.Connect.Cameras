#if SIMPLSHARP
#else
using System.Reflection;
#endif
using System;
using Crestron.SimplSharp.Reflection;
using ICD.Connect.Settings.Core;
using ICD.Connect.Settings;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings.SettingsDevicePropertiesComponents;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings.SettingsDevicePropertiesComponents
{
	public abstract class AbstractSettingsDevicePropertiesComponentPresenter :
		AbstractComponentPresenter<ISettingsDevicePropertiesComponentView>,
		ISettingsDevicePropertiesComponentPresenter
	{
		private PropertySettingsPair m_Model;

		#region Properties

		[CanBeNull]
		protected PropertyInfo Property { get { return m_Model == null ? null : m_Model.Property; } }

		[CanBeNull]
		protected ISettings Settings { get { return m_Model == null ? null : m_Model.Settings; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractSettingsDevicePropertiesComponentPresenter(int room, INavigationController nav,
		                                                             IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		#region Methods

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsDevicePropertiesComponentView view)
		{
			base.Refresh(view);

			string name = Property == null ? string.Empty : StringUtils.NiceName(Property.Name);
			string value = string.Empty;

			if (Settings != null && Property != null)
				value = GetPropertyValueStringRepresentation();

			view.SetPropertyName(name);
			view.SetPropertyValue(value);
		}

		/// <summary>
		/// Sets the property.
		/// </summary>
		/// <param name="model"></param>
		public void SetModel(PropertySettingsPair model)
		{
			if (model == m_Model)
				return;

			m_Model = model;

			RefreshIfVisible();
		}

		#endregion

		/// <summary>
		/// Gets the string representation of the property value for the interface.
		/// E.g. instead of showing device id, show the name of the device.
		/// </summary>
		/// <returns></returns>
		protected abstract string GetPropertyValueStringRepresentation();

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsDevicePropertiesComponentView view)
		{
			base.Subscribe(view);

			view.OnPressed += ViewOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsDevicePropertiesComponentView view)
		{
			base.Unsubscribe(view);

			view.OnPressed -= ViewOnPressed;
		}

		/// <summary>
		/// Called when the user presses the property button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected abstract void ViewOnPressed(object sender, EventArgs eventArgs);

		#endregion
	}
}
