using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings.SettingsDevicePropertiesComponents;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings.SettingsDevicePropertiesComponents
{
	public sealed class SettingsDevicePropertiesDeviceComponentPresenter :
		AbstractSettingsDevicePropertiesListComponentPresenter,
		ISettingsDevicePropertiesDeviceComponentPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsDevicePropertiesDeviceComponentPresenter(int room, INavigationController nav,
		                                                        IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Gets the values for the popup list.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<object> GetListValues()
		{
			ICoreSettings settings = Navigation.LazyLoadPresenter<ISettingsBasePresenter>().SettingsInstance;

			return settings.OriginatorSettings
			               .Where(s => s.OriginatorType.IsAssignableTo(typeof(IDevice)))
			               .Select(d => d.Id)
			               .Cast<int?>()
			               .Order()
			               .Prepend(null)
			               .Cast<object>();
		}

		/// <summary>
		/// Gets the string representation for an item in the popup list.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Item is not correct type.</exception>
		protected override string GetListItemStringRepresentation(object item)
		{
			int? id = (int?)item;
			if (id == null)
				return "Clear";

			ICoreSettings coreSettings = Navigation.LazyLoadPresenter<ISettingsBasePresenter>().SettingsInstance;

			if (!coreSettings.OriginatorSettings.ContainsId((int)id))
				return id.ToString();

			ISettings settings = coreSettings.OriginatorSettings.GetById((int)id);
			return string.Format("{0} - {1}", settings.Name, settings.FactoryName);
		}
	}
}
