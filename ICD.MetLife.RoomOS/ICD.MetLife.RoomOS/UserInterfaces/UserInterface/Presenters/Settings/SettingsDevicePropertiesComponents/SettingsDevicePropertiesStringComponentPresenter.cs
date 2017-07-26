using System;
using System.Globalization;
using ICD.Connect.Settings.Core;
#if SIMPLSHARP
#endif
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings.SettingsDevicePropertiesComponents;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using Activator = Crestron.SimplSharp.Reflection.Activator;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings.SettingsDevicePropertiesComponents
{
	public sealed class SettingsDevicePropertiesStringComponentPresenter :
		AbstractSettingsDevicePropertiesKeyboardComponentPresenter, ISettingsDevicePropertiesStringComponentPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsDevicePropertiesStringComponentPresenter(int room, INavigationController nav,
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

			object value = Property.GetValue(Settings, new object[0]);
			return value == null ? string.Empty : value.ToString();
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

			Type propertyType = Property.PropertyType;
			bool nullable = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
			if (nullable)
				propertyType = Nullable.GetUnderlyingType(propertyType);

			object cast;

			try
			{
				cast = Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				cast = GetDefault();
			}

			Property.SetValue(Settings, cast, null);
			RefreshIfVisible();
		}

		/// <summary>
		/// Gets a default value for the property.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Property is null</exception>
		private object GetDefault()
		{
			if (Property == null)
				throw new InvalidOperationException("Property property is null");

			Type type = Property.PropertyType;
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}
	}
}
