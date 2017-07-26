using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings.SettingsDevicePropertiesComponents;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings.SettingsDevicePropertiesComponents
{
	public sealed class SettingsDevicePropertiesEnumComponentPresenter :
		AbstractSettingsDevicePropertiesListComponentPresenter, ISettingsDevicePropertiesEnumComponentPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsDevicePropertiesEnumComponentPresenter(int room, INavigationController nav,
		                                                      IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Gets the values for the popup list.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Property is null</exception>
		protected override IEnumerable<object> GetListValues()
		{
			if (Property == null)
				throw new InvalidOperationException("Property is null");

			Type type = Property.PropertyType;

			IEnumerable<object> values = EnumUtils.IsFlagsEnum(type)
				                             ? EnumUtils.GetValuesExceptNone(type)
				                             : EnumUtils.GetValues(type);

			return values.OrderBy(e => e.ToString());
		}

		/// <summary>
		/// Gets the string representation for an item in the popup list.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override string GetListItemStringRepresentation(object item)
		{
			return StringUtils.NiceName(item);
		}

		#region List Callbacks

		/// <summary>
		/// Called when the user submits a value via the list.
		/// </summary>
		/// <param name="item"></param>
		protected override void ListOnItemPressed(object item)
		{
			if (Property == null)
				throw new InvalidOperationException("Property is null");

			Type type = Property.PropertyType;

			if (EnumUtils.IsFlagsEnum(type))
				List.SetItemSelected(item, !List.GetItemSelected(item));
			else
				base.ListOnItemPressed(item);
		}

		/// <summary>
		/// Returns true if the given item should be selected in the list.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override bool GetListItemSelected(object item)
		{
			if (Property == null)
				throw new InvalidOperationException("Property is null");

			Type type = Property.PropertyType;

			return EnumUtils.IsFlagsEnum(type)
				       ? EnumUtils.HasFlag(GetObjectValue(), item)
				       : item == GetObjectValue();
		}

		/// <summary>
		/// Called when the list popup is closed.
		/// </summary>
		protected override void ListOnClosed()
		{
			base.ListOnClosed();

			int value = List.GetSelectedItems().Cast<int>().Sum();
			SetObjectValue(value);
		}

		#endregion
	}
}
