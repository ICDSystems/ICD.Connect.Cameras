using System;
using System.Collections.Generic;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings.SettingsDevicePropertiesComponents
{
	public abstract class AbstractSettingsDevicePropertiesListComponentPresenter :
		AbstractSettingsDevicePropertiesComponentPresenter
	{
		private IPopupListPresenter m_PopupList;

		/// <summary>
		/// Gets the popup list menu.
		/// </summary>
		protected IPopupListPresenter List
		{
			get { return m_PopupList ?? (m_PopupList = Navigation.LazyLoadPresenter<IPopupListPresenter>()); }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractSettingsDevicePropertiesListComponentPresenter(int room, INavigationController nav,
		                                                                 IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Gets the string representation of the property value for the interface.
		/// E.g. instead of showing device id, show the name of the device.
		/// </summary>
		/// <returns></returns>
		protected override string GetPropertyValueStringRepresentation()
		{
			object value = GetObjectValue();
			return value == null ? string.Empty : GetListItemStringRepresentation(value);
		}

		/// <summary>
		/// Called when the user presses the property button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			PopupList();
		}

		/// <summary>
		/// Gets the values for the popup list.
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerable<object> GetListValues();

		/// <summary>
		/// Gets the string representation for an item in the popup list.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected abstract string GetListItemStringRepresentation(object item);

		/// <summary>
		/// Returns true if the given item should be selected in the list.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual bool GetListItemSelected(object item)
		{
			return item == GetObjectValue();
		}

		/// <summary>
		/// Shows the available values.
		/// </summary>
		private void PopupList()
		{
			string title = GetListTitle();
			IEnumerable<object> values = GetListValues();

			List.SetListItems(values, GetListItemStringRepresentation, GetListItemSelected, title, ListOnItemPressed, ListOnClosed);
			List.ShowView(true);
		}

		#region List Callbacks

		/// <summary>
		/// Gets the title for the popup list.
		/// </summary>
		/// <returns></returns>
		private string GetListTitle()
		{
			return Property == null ? string.Empty : StringUtils.NiceName(Property.Name);
		}

		/// <summary>
		/// Called when the user submits a value via the list.
		/// </summary>
		/// <param name="item"></param>
		protected virtual void ListOnItemPressed(object item)
		{
			List.ShowView(false);
			SetObjectValue(item);
		}

		/// <summary>
		/// Called when the list popup is closed.
		/// </summary>
		protected virtual void ListOnClosed()
		{
		}

		#endregion

		/// <summary>
		/// Sets the property value.
		/// </summary>
		/// <param name="value"></param>
		/// <exception cref="InvalidOperationException">Property is null</exception>
		/// <exception cref="InvalidOperationException">Settings is null</exception>
		protected void SetObjectValue(object value)
		{
			if (Property == null)
				throw new InvalidOperationException("Property property is null");
			if (Settings == null)
				throw new InvalidOperationException("Settings property is null");

			// If the value is null (clear item), use the default value
			value = value ?? ReflectionUtils.GetDefaultValue(Property.PropertyType);

			Property.SetValue(Settings, value, null);

			RefreshIfVisible();
		}

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Property is null</exception>
		/// <exception cref="InvalidOperationException">Settings is null</exception>
		protected object GetObjectValue()
		{
			if (Property == null)
				throw new InvalidOperationException("Property property is null");
			if (Settings == null)
				throw new InvalidOperationException("Settings property is null");

			return Property.GetValue(Settings, new object[0]);
		}
	}
}
