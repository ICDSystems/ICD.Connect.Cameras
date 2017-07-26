using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Volume;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Volume
{
	public sealed partial class VolumeComponentView : AbstractComponentView, IVolumeComponentView
	{
		public event EventHandler OnVolumeUpButtonPressed;
		public event EventHandler OnVolumeDownButtonPressed;
		public event EventHandler OnVolumeButtonReleased;
		public event EventHandler OnMuteButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public VolumeComponentView(ISigInputOutput panel, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnVolumeUpButtonPressed = null;
			OnVolumeDownButtonPressed = null;
			OnVolumeButtonReleased = null;
			OnMuteButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the value on the volume bar in the range 0.0f - 1.0f
		/// </summary>
		/// <param name="volume"></param>
		public void SetVolumePercentage(float volume)
		{
			m_Guage.SetValuePercentage(volume);
		}

		/// <summary>
		/// Sets the title text.
		/// </summary>
		/// <param name="title"></param>
		public void SetTitle(string title)
		{
			m_Label.SetLabelTextAtJoin(m_Label.SerialLabelJoins.First(), title);
		}

		/// <summary>
		/// Sets the enabled state of the volume guage.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetGuageEnabled(bool enabled)
		{
			m_Guage.Enable(enabled);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_VolumeUpButton.OnPressed += VolumeUpButtonOnPressed;
			m_VolumeUpButton.OnReleased += VolumeUpButtonOnReleased;
			m_VolumeDownButton.OnPressed += VolumeDownButtonOnPressed;
			m_VolumeDownButton.OnReleased += VolumeDownButtonOnReleased;
			m_MuteButton.OnPressed += MuteButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_VolumeUpButton.OnPressed -= VolumeUpButtonOnPressed;
			m_VolumeUpButton.OnReleased -= VolumeUpButtonOnReleased;
			m_VolumeDownButton.OnPressed -= VolumeDownButtonOnPressed;
			m_VolumeDownButton.OnReleased -= VolumeDownButtonOnReleased;
			m_MuteButton.OnPressed -= MuteButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the mute button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void MuteButtonOnPressed(object sender, EventArgs args)
		{
			OnMuteButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user releases the volume down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VolumeDownButtonOnReleased(object sender, EventArgs args)
		{
			OnVolumeButtonReleased.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the volume down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VolumeDownButtonOnPressed(object sender, EventArgs args)
		{
			OnVolumeDownButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user releases the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VolumeUpButtonOnReleased(object sender, EventArgs args)
		{
			OnVolumeButtonReleased.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VolumeUpButtonOnPressed(object sender, EventArgs args)
		{
			OnVolumeUpButtonPressed.Raise(this);
		}

		#endregion
	}
}
