using System;
using ICD.Common.Properties;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings
{
	public interface ISettingsDeviceStatusView : IView
	{
		/// <summary>
		/// Raised when the user presses a button.
		/// </summary>
		[PublicAPI]
		event EventHandler<UShortEventArgs> OnButtonPressed;

		/// <summary>
		/// Sets the title label text.
		/// </summary>
		/// <param name="title"></param>
		[PublicAPI]
		void SetTitleLabel(string title);

		/// <summary>
		/// Sets the number of buttons.
		/// </summary>
		/// <param name="count"></param>
		[PublicAPI]
		void SetButtonsCount(ushort count);

		/// <summary>
		/// Sets the label for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		[PublicAPI]
		void SetButtonLabel(ushort index, string label);

		/// <summary>
		/// Sets the icon for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		[PublicAPI]
		void SetButtonIcon(ushort index, string icon);
	}
}
