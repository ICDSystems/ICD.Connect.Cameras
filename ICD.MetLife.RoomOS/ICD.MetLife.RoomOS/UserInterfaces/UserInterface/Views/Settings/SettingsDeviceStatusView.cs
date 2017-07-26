using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsDeviceStatusView : AbstractView, ISettingsDeviceStatusView
	{
		public event EventHandler<UShortEventArgs> OnButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public SettingsDeviceStatusView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the title label text.
		/// </summary>
		/// <param name="title"></param>
		public void SetTitleLabel(string title)
		{
			m_TitleLabel.SetLabelTextAtJoin(m_TitleLabel.SerialLabelJoins.First(), title);
		}

		/// <summary>
		/// Sets the number of buttons.
		/// </summary>
		/// <param name="count"></param>
		public void SetButtonsCount(ushort count)
		{
			count = Math.Min(count, m_ButtonList.MaxSize);
			m_ButtonList.SetNumberOfItems(count);
		}

		/// <summary>
		/// Sets the label for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		public void SetButtonLabel(ushort index, string label)
		{
			if (index < m_ButtonList.MaxSize)
				m_ButtonList.SetItemLabel(index, label);
		}

		/// <summary>
		/// Sets the icon for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		public void SetButtonIcon(ushort index, string icon)
		{
			if (index < m_ButtonList.MaxSize)
				m_ButtonList.SetItemIcon(index, icon);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ButtonList.OnButtonClicked += ButtonListOnButtonClicked;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ButtonList.OnButtonClicked -= ButtonListOnButtonClicked;
		}

		/// <summary>
		/// Called when the user presses a button in the button list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ButtonListOnButtonClicked(object sender, UShortEventArgs args)
		{
			OnButtonPressed.Raise(this, new UShortEventArgs(args.Data));
		}

		#endregion
	}
}
