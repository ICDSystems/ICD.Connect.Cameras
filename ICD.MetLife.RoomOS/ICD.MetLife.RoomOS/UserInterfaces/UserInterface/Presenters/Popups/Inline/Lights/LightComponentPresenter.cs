using System;
using ICD.Connect.Lighting;
using ICD.Connect.Lighting.EventArguments;
using ICD.Connect.Rooms;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils.Extensions;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Lights;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Lights
{
	public sealed class LightComponentPresenter : AbstractComponentPresenter<ILightComponentView>, ILightComponentPresenter
	{
		public event EventHandler OnButtonPressed;
		public event EventHandler OnButtonReleased;

		private LightingProcessorControl m_Control;
		private ILightingProcessorDevice m_LightingProcessor;

		#region Properties

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
		public LightComponentPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnButtonPressed = null;
			OnButtonReleased = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ILightComponentView view)
		{
			base.Refresh(view);

			float percentage = m_LightingProcessor.GetLoadLevel(m_Control);

			view.SetTitle(m_Control.Name);
			view.SetPercentage(percentage);
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(MetlifeRoom room)
		{
			base.Subscribe(room);

			m_LightingProcessor = room == null ? null : room.GetDevice<ILightingProcessorDevice>();
			if (m_LightingProcessor == null)
				return;

			m_LightingProcessor.OnRoomLoadLevelChanged += LightingProcessorOnRoomLoadLevelChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (m_LightingProcessor == null)
				return;

			m_LightingProcessor.OnRoomLoadLevelChanged -= LightingProcessorOnRoomLoadLevelChanged;
		}

		/// <summary>
		/// Called when a load level changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LightingProcessorOnRoomLoadLevelChanged(object sender, RoomLoadLevelEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ILightComponentView view)
		{
			base.Subscribe(view);

			view.OnUpButtonPressed += ViewOnUpButtonPressed;
			view.OnDownButtonPressed += ViewOnDownButtonPressed;
			view.OnButtonReleased += ViewOnButtonReleased;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ILightComponentView view)
		{
			base.Unsubscribe(view);

			view.OnUpButtonPressed -= ViewOnUpButtonPressed;
			view.OnDownButtonPressed -= ViewOnDownButtonPressed;
			view.OnButtonReleased -= ViewOnButtonReleased;
		}

		/// <summary>
		/// Called when the used releases a button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnButtonReleased(object sender, EventArgs eventArgs)
		{
			m_LightingProcessor.StopRampingLoadLevel(Control);
			OnButtonReleased.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDownButtonPressed(object sender, EventArgs eventArgs)
		{
			m_LightingProcessor.StartLoweringLoadLevel(Control);
			OnButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnUpButtonPressed(object sender, EventArgs eventArgs)
		{
			m_LightingProcessor.StartRaisingLoadLevel(Control);
			OnButtonPressed.Raise(this);
		}

		#endregion
	}
}
