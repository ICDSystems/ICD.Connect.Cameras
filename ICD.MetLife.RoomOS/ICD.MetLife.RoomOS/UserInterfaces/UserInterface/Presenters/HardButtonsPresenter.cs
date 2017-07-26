using System;
using ICD.Connect.Lighting;
using ICD.Connect.Rooms;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Home;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Lights;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Volume;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters
{
	public sealed class HardButtonsPresenter : AbstractPresenter<IHardButtonsView>, IHardButtonsPresenter
	{
		private IVolumeSidePresenter m_CachedVolumePresenter;

		/// <summary>
		/// Gets the popup volume presenter.
		/// </summary>
		private IVolumeSidePresenter VolumePresenter
		{
			get
			{
				return m_CachedVolumePresenter ?? (m_CachedVolumePresenter = Navigation.LazyLoadPresenter<IVolumeSidePresenter>());
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public HardButtonsPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IHardButtonsView view)
		{
			base.Subscribe(view);

			view.OnPowerButtonPressed += ViewOnPowerButtonPressed;
			view.OnHomeButtonPressed += ViewOnHomeButtonPressed;
			view.OnLightsButtonPressed += ViewOnLightsButtonPressed;
			view.OnVolumeUpButtonPressed += ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed += ViewOnVolumeDownButtonPressed;
			view.OnVolumeButtonReleased += ViewOnVolumeButtonReleased;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IHardButtonsView view)
		{
			base.Unsubscribe(view);

			view.OnPowerButtonPressed -= ViewOnPowerButtonPressed;
			view.OnHomeButtonPressed -= ViewOnHomeButtonPressed;
			view.OnLightsButtonPressed -= ViewOnLightsButtonPressed;
			view.OnVolumeUpButtonPressed -= ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed -= ViewOnVolumeDownButtonPressed;
			view.OnVolumeButtonReleased -= ViewOnVolumeButtonReleased;
		}

		/// <summary>
		/// Called when the user presses the power button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnPowerButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			if (Room.ShutdownTimer.IsRunning)
				Room.ShutdownTimer.Stop();
			else
				Room.ResetShutdownTimer();
		}

		/// <summary>
		/// Called when the user presses the home button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnHomeButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.NavigateTo<IInACallPresenter>();
		}

		/// <summary>
		/// Called when the user presses the lights button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnLightsButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room.GetDevice<ILightingProcessorDevice>() == null)
				return;

			ILightsPresenter presenter = Navigation.LazyLoadPresenter<ILightsPresenter>();
			presenter.ShowView(!presenter.IsViewVisible);
		}

		/// <summary>
		/// Called when the user presses the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeUpButtonPressed(object sender, EventArgs eventArgs)
		{
			VolumePresenter.VolumeControl = Room == null ? null : Room.GetBestVolumeControlForContext();
			if (VolumePresenter.VolumeControl != null)
				VolumePresenter.VolumeUp();
		}

		/// <summary>
		/// Called when the user presses the volume down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeDownButtonPressed(object sender, EventArgs eventArgs)
		{
			VolumePresenter.VolumeControl = Room == null ? null : Room.GetBestVolumeControlForContext();
			if (VolumePresenter.VolumeControl != null)
				VolumePresenter.VolumeDown();
		}

		/// <summary>
		/// Called when the user releases a volume button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeButtonReleased(object sender, EventArgs eventArgs)
		{
			VolumePresenter.Release();
		}

		#endregion
	}
}
