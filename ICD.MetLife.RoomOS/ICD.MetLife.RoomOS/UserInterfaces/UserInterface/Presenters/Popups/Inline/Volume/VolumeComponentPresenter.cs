using System;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Timers;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Volume;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Volume;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Volume
{
	public sealed class VolumeComponentPresenter : AbstractComponentPresenter<IVolumeComponentView>,
	                                               IVolumeComponentPresenter
	{
		private const ushort INITIAL_INCREMENT = 1;
		private const ushort REPEAT_INCREMENT = 1;
		private const ushort BEFORE_REPEAT_MILLISECONDS = 500;
		private const ushort BETWEEN_REPEAT_MILLISECONDS = 100;

		public event EventHandler OnVolumeUpButtonPressed;
		public event EventHandler OnVolumeDownButtonPressed;
		public event EventHandler OnVolumeButtonReleased;
		public event EventHandler OnMuteButtonPressed;

		private readonly VolumeRepeater m_Repeater;
		private IVolumeDeviceControl m_VolumeControl;

		/// <summary>
		/// Gets/sets the volume device.
		/// </summary>
		public IVolumeDeviceControl VolumeControl
		{
			get { return m_VolumeControl; }
			set
			{
				if (value == m_VolumeControl)
					return;

				Unsubscribe(m_VolumeControl);
				m_VolumeControl = value;
				Subscribe(m_VolumeControl);

				m_Repeater.Release();
				m_Repeater.SetControl(m_VolumeControl);

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public VolumeComponentPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_Repeater = new VolumeRepeater(INITIAL_INCREMENT, REPEAT_INCREMENT, BEFORE_REPEAT_MILLISECONDS,
			                                BETWEEN_REPEAT_MILLISECONDS);
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
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVolumeComponentView view)
		{
			base.Refresh(view);

			string label = VolumeControl == null ? string.Empty : VolumeControl.Name;
			bool enableGuage = VolumeControl != null && !VolumeControl.IsMuted;
			float volume = VolumeControl == null ? 0 : VolumeControl.GetRawVolumeAsSafetyPercentage();

			view.SetTitle(label);
			view.SetGuageEnabled(enableGuage);
			view.SetVolumePercentage(volume);
		}

		/// <summary>
		/// Begins ramping the device volume up.
		/// </summary>
		public void VolumeUp()
		{
			if (VolumeControl == null)
				return;

			ShowView(true);
			m_Repeater.VolumeUpHold();
		}

		/// <summary>
		/// Begins ramping the device volume down.
		/// </summary>
		public void VolumeDown()
		{
			if (VolumeControl == null)
				return;

			ShowView(true);
			m_Repeater.VolumeDownHold();
		}

		/// <summary>
		/// Stops ramping the device volume.
		/// </summary>
		public void Release()
		{
			if (VolumeControl == null)
				return;

			m_Repeater.Release();
		}

		/// <summary>
		/// Toggles the mute state of the device.
		/// </summary>
		public void ToggleMute()
		{
			if (VolumeControl == null)
				return;

			ShowView(true);
			VolumeControl.MuteToggle();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVolumeComponentView view)
		{
			base.Subscribe(view);

			view.OnMuteButtonPressed += ViewOnMuteButtonPressed;
			view.OnVolumeButtonReleased += ViewOnVolumeButtonReleased;
			view.OnVolumeUpButtonPressed += ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed += ViewOnVolumeDownButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVolumeComponentView view)
		{
			base.Unsubscribe(view);

			view.OnMuteButtonPressed -= ViewOnMuteButtonPressed;
			view.OnVolumeButtonReleased -= ViewOnVolumeButtonReleased;
			view.OnVolumeUpButtonPressed -= ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed -= ViewOnVolumeDownButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the volume down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeDownButtonPressed(object sender, EventArgs eventArgs)
		{
			VolumeDown();

			OnVolumeDownButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeUpButtonPressed(object sender, EventArgs eventArgs)
		{
			VolumeUp();

			OnVolumeUpButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user releases a volume button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeButtonReleased(object sender, EventArgs eventArgs)
		{
			Release();

			OnVolumeButtonReleased.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the mute button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnMuteButtonPressed(object sender, EventArgs eventArgs)
		{
			ToggleMute();

			OnMuteButtonPressed.Raise(this);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribe to the control events.
		/// </summary>
		/// <param name="control"></param>
		private void Subscribe(IVolumeDeviceControl control)
		{
			if (control == null)
				return;

			control.OnMuteStateChanged += DeviceOnMuteStateChanged;
			control.OnRawVolumeChanged += DeviceOnVolumeChanged;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="control"></param>
		private void Unsubscribe(IVolumeDeviceControl control)
		{
			if (control == null)
				return;

			control.OnMuteStateChanged -= DeviceOnMuteStateChanged;
			control.OnRawVolumeChanged -= DeviceOnVolumeChanged;
		}

		/// <summary>
		/// Called when the control volume changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DeviceOnVolumeChanged(object sender, FloatEventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the control mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DeviceOnMuteStateChanged(object sender, BoolEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
