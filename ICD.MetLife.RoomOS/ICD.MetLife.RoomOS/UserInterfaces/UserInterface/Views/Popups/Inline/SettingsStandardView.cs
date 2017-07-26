using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Connect.UI.EventArguments;
using ICD.Connect.UI.Utils;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline
{
	public sealed partial class SettingsStandardView : AbstractView, ISettingsStandardView
	{
		public event EventHandler OnEnterButtonPressed;
		public event EventHandler OnClearButtonPressed;
		public event EventHandler<CharEventArgs> OnKeypadButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public SettingsStandardView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnEnterButtonPressed = null;
			OnClearButtonPressed = null;
			OnKeypadButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the text in the password text field.
		/// </summary>
		/// <param name="password"></param>
		public void SetPasswordText(string password)
		{
			password = StringUtils.Repeat('*', (password ?? string.Empty).Length);
			m_PasswordEntry.SetLabelTextAtJoin(m_PasswordEntry.SerialLabelJoins.First(), password);
		}

		/// <summary>
		/// Sets the number of devices.
		/// </summary>
		/// <param name="count"></param>
		public void SetDeviceCount(ushort count)
		{
			count = Math.Min(count, m_DeviceList.MaxSize);
			m_DeviceList.SetNumberOfItems(count);
		}

		/// <summary>
		/// Sets the label for the device at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="name"></param>
		/// <param name="color"></param>
		public void SetDeviceLabel(ushort index, string name, eColor color)
		{
			if (index >= m_DeviceList.MaxSize)
				return;

			string hex = VtProColors.ColorToHexString(color);
			string label = HtmlUtils.FormatColoredText(name, hex);

			m_DeviceList.SetItemVisible(index, true);
			m_DeviceList.SetItemLabel(index, label);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Keypad.OnButtonPressed += KeypadOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Keypad.OnButtonPressed -= KeypadOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses a keypad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void KeypadOnButtonPressed(object sender, SimpleKeypadEventArgs args)
		{
			switch (args.Data)
			{
				case SimpleKeypadEventArgs.eButton.MiscButtonOne:
					OnClearButtonPressed.Raise(this);
					break;

				case SimpleKeypadEventArgs.eButton.MiscButtonTwo:
					OnEnterButtonPressed.Raise(this);
					break;

				default:
					char character = m_Keypad.GetButtonChar(args.Data);
					OnKeypadButtonPressed.Raise(this, new CharEventArgs(character));
					break;
			}
		}

		#endregion
	}
}
