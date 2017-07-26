using ICD.Common.Properties;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using System;
using System.Linq;
using ICD.Common.Attributes.Properties;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings.SettingsDevicePropertiesComponents;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsDevicePropertiesComponentPresenterFactory :
		AbstractListItemFactory
			<PropertySettingsPair, ISettingsDevicePropertiesComponentPresenter, ISettingsDevicePropertiesComponentView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public SettingsDevicePropertiesComponentPresenterFactory(INavigationController navigationController,
		                                                         ListItemFactory<ISettingsDevicePropertiesComponentView>
			                                                         viewFactory)
			: base(navigationController, viewFactory)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(PropertySettingsPair model, ISettingsDevicePropertiesComponentPresenter presenter,
		                                     ISettingsDevicePropertiesComponentView view)
		{
			presenter.SetView(view);
			presenter.SetModel(model);
		}

		/// <summary>
		/// Gets the presenter type for the given model instance.
		/// Override to fill lists with different presenters.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		protected override Type GetPresenterTypeForModel(PropertySettingsPair model)
		{
			SettingsProperty.ePropertyType propertyType = GetPropertyType(model.Property);
			return PresenterTypeForPropertyType(propertyType);
		}

		/// <summary>
		/// Gets the property type based on attributes.
		/// </summary>
		/// <returns></returns>
		public static SettingsProperty.ePropertyType GetPropertyType(PropertyInfo property)
		{
			if (property.PropertyType.IsEnum)
				return SettingsProperty.ePropertyType.Enum;

			SettingsProperty attribute = property.GetCustomAttributes<SettingsProperty>(true).FirstOrDefault();
			return attribute == null
				       ? SettingsProperty.ePropertyType.Default
				       : attribute.PropertyType;
		}

		/// <summary>
		/// Returns the component presenter type for the given property type.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[CanBeNull]
		private static Type PresenterTypeForPropertyType(SettingsProperty.ePropertyType key)
		{
			switch (key)
			{
				case SettingsProperty.ePropertyType.Default:
					return typeof(ISettingsDevicePropertiesStringComponentPresenter);
				case SettingsProperty.ePropertyType.PortId:
					return typeof(ISettingsDevicePropertiesPortComponentPresenter);
				case SettingsProperty.ePropertyType.DeviceId:
					return typeof(ISettingsDevicePropertiesDeviceComponentPresenter);
				case SettingsProperty.ePropertyType.Ipid:
					return typeof(ISettingsDevicePropertiesIpidComponentPresenter);
				case SettingsProperty.ePropertyType.Enum:
					return typeof(ISettingsDevicePropertiesEnumComponentPresenter);
				default:
					throw new ArgumentOutOfRangeException("key");
			}
		}
	}
}
