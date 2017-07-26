using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Rooms;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Volume;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Volume;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Volume
{
	public sealed class VolumePresenter : AbstractPopupPresenter<IVolumeView>, IVolumePresenter
	{
		private const ushort HIDE_TIME = 30 * 1000;

		private readonly VolumeComponentPresenterFactory m_VolumeComponentFactory;
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_VisibilityTimer;

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Volume"; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public VolumePresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_VolumeComponentFactory = new VolumeComponentPresenterFactory(nav, VolumeComponentViewFactory);
			m_RefreshSection = new SafeCriticalSection();
			m_VisibilityTimer = SafeTimer.Stopped(() => ShowView(false));
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_VisibilityTimer.Dispose();

			UnsubscribeVolumeComponents();
			m_VolumeComponentFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVolumeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				UnsubscribeVolumeComponents();

				IEnumerable<IVolumeDeviceControl> controls = Room.GetControls<IVolumeDeviceControl>();
				foreach (IVolumeComponentPresenter presenter in m_VolumeComponentFactory.BuildChildren(controls))
				{
					Subscribe(presenter);
					presenter.ShowView(true);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Stops the visibility timer.
		/// </summary>
		[PublicAPI]
		public void StopVisibilityTimer()
		{
			m_VisibilityTimer.Stop();
		}

		/// <summary>
		/// Resets the visibility timer.
		/// </summary>
		[PublicAPI]
		public void ResetVisibilityTimer()
		{
			m_VisibilityTimer.Reset(HIDE_TIME);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Generates the given number of volume component views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IVolumeComponentView> VolumeComponentViewFactory(ushort count)
		{
			return GetView().GetChildVolumeViews(ViewFactory, count);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVolumeView view)
		{
			base.Subscribe(view);

			view.OnScrollingChanged += ViewOnScrollingChanged;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVolumeView view)
		{
			base.Unsubscribe(view);

			view.OnScrollingChanged -= ViewOnScrollingChanged;
		}

		private void ViewOnScrollingChanged(object sender, BoolEventArgs args)
		{
			if (args.Data)
				StopVisibilityTimer();
			else
				ResetVisibilityTimer();
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			ResetVisibilityTimer();
		}

		#endregion

		#region Volume Component Callbacks

		/// <summary>
		/// Unsubscribe from all of the volume component events.
		/// </summary>
		private void UnsubscribeVolumeComponents()
		{
			foreach (IVolumeComponentPresenter presenter in m_VolumeComponentFactory)
				Unsubscribe(presenter);
		}

		private void Subscribe(IVolumeComponentPresenter presenter)
		{
			if (presenter == null)
				return;

			presenter.OnMuteButtonPressed += PresenterOnMuteButtonPressed;
			presenter.OnVolumeButtonReleased += PresenterOnVolumeButtonReleased;
			presenter.OnVolumeUpButtonPressed += PresenterOnVolumeUpButtonPressed;
			presenter.OnVolumeDownButtonPressed += PresenterOnVolumeDownButtonPressed;
		}

		private void Unsubscribe(IVolumeComponentPresenter presenter)
		{
			if (presenter == null)
				return;

			presenter.OnMuteButtonPressed -= PresenterOnMuteButtonPressed;
			presenter.OnVolumeButtonReleased -= PresenterOnVolumeButtonReleased;
			presenter.OnVolumeUpButtonPressed -= PresenterOnVolumeUpButtonPressed;
			presenter.OnVolumeDownButtonPressed -= PresenterOnVolumeDownButtonPressed;
		}

		private void PresenterOnVolumeDownButtonPressed(object sender, EventArgs eventArgs)
		{
			StopVisibilityTimer();
		}

		private void PresenterOnVolumeUpButtonPressed(object sender, EventArgs eventArgs)
		{
			StopVisibilityTimer();
		}

		private void PresenterOnVolumeButtonReleased(object sender, EventArgs eventArgs)
		{
			ResetVisibilityTimer();
		}

		private void PresenterOnMuteButtonPressed(object sender, EventArgs eventArgs)
		{
			ResetVisibilityTimer();
		}

		#endregion
	}
}
