using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings
{
	public interface ISettingsDevicePropertiesComponentView : IView
	{
		/// <summary>
		/// Raised when the user presses the property.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Sets the property name label text.
		/// </summary>
		/// <param name="name"></param>
		void SetPropertyName(string name);

		/// <summary>
		/// Sets the property value label text.
		/// </summary>
		/// <param name="value"></param>
		void SetPropertyValue(string value);
	}
}
