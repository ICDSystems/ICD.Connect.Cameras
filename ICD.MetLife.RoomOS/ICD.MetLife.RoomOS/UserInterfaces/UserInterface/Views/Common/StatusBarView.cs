using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Common;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Common
{
	public sealed partial class StatusBarView : AbstractView, IStatusBarView
	{
		private const ushort MODE_BLACK = 0;
		private const ushort MODE_GREEN = 1;
		private const ushort MODE_RED = 2;
		private const ushort MODE_YELLOW = 3;

		public event EventHandler OnAudioButtonPressed;
		public event EventHandler OnSettingsButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public StatusBarView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnAudioButtonPressed = null;
			OnSettingsButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the visibility of the view.
		/// </summary>
		/// <param name="visible"></param>
		public override void Show(bool visible)
		{
			// Always visible
		}

		/// <summary>
		/// Sets the room name text.
		/// </summary>
		/// <param name="text"></param>
		public void SetRoomNameText(string text)
		{
			m_RoomNameLabel.SetLabelTextAtJoin(m_RoomNameLabel.SerialLabelJoins.Skip(1).First(), text);
		}

		/// <summary>
		/// Sets the room prefix text.
		/// </summary>
		/// <param name="text"></param>
		public void SetRoomPrefixText(string text)
		{
			m_RoomNameLabel.SetLabelTextAtJoin(m_RoomNameLabel.SerialLabelJoins.First(), text);
		}

		/// <summary>
		/// Sets the color of the status bar.
		/// </summary>
		/// <param name="color"></param>
		public void SetColor(eColor color)
		{
			ushort mode = GetModeForColor(color);
			m_BackgroundImage.SetMode(mode);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_AudioButton.OnPressed += AudioButtonOnPressed;
			m_SettingsButton.OnPressed += SettingsButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_AudioButton.OnPressed -= AudioButtonOnPressed;
			m_SettingsButton.OnPressed -= SettingsButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the settings button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SettingsButtonOnPressed(object sender, EventArgs args)
		{
			OnSettingsButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the audio button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void AudioButtonOnPressed(object sender, EventArgs args)
		{
			OnAudioButtonPressed.Raise(this);
		}

		/// <summary>
		/// Gets the mode for the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		private static ushort GetModeForColor(eColor color)
		{
			switch (color)
			{
				case eColor.Default:
					return MODE_BLACK;
				case eColor.Green:
					return MODE_GREEN;
				case eColor.Yellow:
					return MODE_YELLOW;
				case eColor.Red:
					return MODE_RED;
				default:
					throw new ArgumentOutOfRangeException("color");
			}
		}

		#endregion
	}
}
