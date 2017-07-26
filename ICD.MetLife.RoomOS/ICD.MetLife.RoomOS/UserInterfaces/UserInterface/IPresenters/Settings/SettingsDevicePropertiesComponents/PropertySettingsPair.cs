#if SIMPLSHARP
#else
using System.Reflection;
#endif
using System;
using Crestron.SimplSharp.Reflection;
using ICD.Connect.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings.SettingsDevicePropertiesComponents
{
	public sealed class PropertySettingsPair
	{
		private readonly PropertyInfo m_Property;
		private readonly ISettings m_Settings;

		public PropertyInfo Property { get { return m_Property; } }
		public ISettings Settings { get { return m_Settings; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="settings"></param>
		public PropertySettingsPair(PropertyInfo property, ISettings settings)
		{
			if (property == null)
				throw new ArgumentNullException("property");

			if (settings == null)
				throw new ArgumentNullException("settings");

			if (!property.DeclaringType.IsInstanceOfType(settings))
				throw new InvalidOperationException("Property does not belong to given settings instance");

			m_Property = property;
			m_Settings = settings;
		}
	}
}