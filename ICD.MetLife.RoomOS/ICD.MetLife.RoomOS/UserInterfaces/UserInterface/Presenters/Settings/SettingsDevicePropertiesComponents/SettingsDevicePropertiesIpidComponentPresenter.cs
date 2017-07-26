using System;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings.SettingsDevicePropertiesComponents;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings.SettingsDevicePropertiesComponents
{
	public sealed class SettingsDevicePropertiesIpidComponentPresenter :
		AbstractSettingsDevicePropertiesKeyboardComponentPresenter, ISettingsDevicePropertiesIpidComponentPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsDevicePropertiesIpidComponentPresenter(int room, INavigationController nav,
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
			return GetStringValue();
		}

		/// <summary>
		/// Gets the value from the property.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Property is null</exception>
		/// <exception cref="InvalidOperationException">Settings is null</exception>
		protected override string GetStringValue()
		{
			if (Property == null)
				throw new InvalidOperationException("Property property is null");
			if (Settings == null)
				throw new InvalidOperationException("Settings property is null");

			byte ipid = (byte)Property.GetValue(Settings, new object[0]);
			return StringUtils.ToIpIdString(ipid);
		}

		/// <summary>
		/// Converts and sets the value to the property.
		/// </summary>
		/// <param name="value"></param>
		/// <exception cref="InvalidOperationException">Property is null</exception>
		/// <exception cref="InvalidOperationException">Settings is null</exception>
		protected override void SetStringValue(string value)
		{
			if (Property == null)
				throw new InvalidOperationException("Property property is null");
			if (Settings == null)
				throw new InvalidOperationException("Settings property is null");

			byte ipid;

			try
			{
				ipid = StringUtils.FromIpIdString(value);
			}
			catch (FormatException)
			{
				ipid = 0;
			}
			catch (OverflowException)
			{
				ipid = 0;
			}

			Property.SetValue(Settings, ipid, null);
			RefreshIfVisible();
		}
	}
}
