using System;
using ICD.Connect.Lighting;
using ICD.Connect.Rooms;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Lights;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Lights
{
	public sealed class ShadeComponentPresenter : AbstractComponentPresenter<IShadeComponentView>, IShadeComponentPresenter
	{
		public event EventHandler OnUpButtonPressed;
		public event EventHandler OnDownButtonPressed;
		public event EventHandler OnStopButtonPressed;

		private ILightingProcessorDevice m_LightingProcessor;
		private LightingProcessorControl m_Control;

		#region Properties

		/// <summary>
		/// Gets the room lighting processor.
		/// </summary>
		private ILightingProcessorDevice LightingProcessor
		{
			get { return m_LightingProcessor ?? (m_LightingProcessor = Room.GetDevice<ILightingProcessorDevice>()); }
		}

		/// <summary>
		/// Gets/sets the lighting control.
		/// </summary>
		public LightingProcessorControl Control
		{
			get { return m_Control; }
			set
			{
				if (value == m_Control)
					return;

				m_Control = value;

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public ShadeComponentPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnUpButtonPressed = null;
			OnDownButtonPressed = null;
			OnStopButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IShadeComponentView view)
		{
			base.Refresh(view);

			view.SetTitle(m_Control.Name);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IShadeComponentView view)
		{
			base.Subscribe(view);

			view.OnUpButtonPressed += ViewOnUpButtonPressed;
			view.OnDownButtonPressed += ViewOnDownButtonPressed;
			view.OnStopButtonPressed += ViewOnStopButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IShadeComponentView view)
		{
			base.Unsubscribe(view);

			view.OnUpButtonPressed -= ViewOnUpButtonPressed;
			view.OnDownButtonPressed -= ViewOnDownButtonPressed;
			view.OnStopButtonPressed -= ViewOnStopButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the stop button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnStopButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (Control.ControlType)
			{
				case LightingProcessorControl.eControlType.Shade:
					LightingProcessor.StopMovingShade(Control.Room, Control.Id);
					break;
				case LightingProcessorControl.eControlType.ShadeGroup:
					LightingProcessor.StopMovingShadeGroup(Control.Room, Control.Id);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			OnStopButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDownButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (Control.ControlType)
			{
				case LightingProcessorControl.eControlType.Shade:
					LightingProcessor.StartLoweringShade(Control.Room, Control.Id);
					break;
				case LightingProcessorControl.eControlType.ShadeGroup:
					LightingProcessor.StartLoweringShadeGroup(Control.Room, Control.Id);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			OnDownButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnUpButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (Control.ControlType)
			{
				case LightingProcessorControl.eControlType.Shade:
					LightingProcessor.StartRaisingShade(Control.Room, Control.Id);
					break;
				case LightingProcessorControl.eControlType.ShadeGroup:
					LightingProcessor.StartRaisingShadeGroup(Control.Room, Control.Id);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			OnUpButtonPressed.Raise(this);
		}

		#endregion
	}
}
