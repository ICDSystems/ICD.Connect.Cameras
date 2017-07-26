using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Lighting;
using ICD.Connect.Lighting.EventArguments;
using ICD.Connect.Rooms;
using ICD.Connect.Settings.Core;
using ICD.Common.Properties;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Lights;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Inline.Lights
{
	public sealed class LightsPresenter : AbstractPopupPresenter<ILightsView>, ILightsPresenter
	{
		private const ushort HIDE_TIME = 30 * 1000;

		private readonly LightComponentPresenterFactory m_LightComponentFactory;
		private readonly ShadeComponentPresenterFactory m_ShadeComponentFactory;
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_VisibilityTimer;

		private ILightingProcessorDevice m_LightingProcessor;
		private LightingProcessorControl[] m_Presets;
		private ILightComponentPresenter[] m_LightPresenters;

		#region Properties

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Lights"; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public LightsPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_LightComponentFactory = new LightComponentPresenterFactory(nav, LightComponentViewFactory);
			m_ShadeComponentFactory = new ShadeComponentPresenterFactory(nav, ShadeComponentViewFactory);

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

			UnsubscribeLightComponents();
			UnsubscribeShadeComponents();

			m_LightComponentFactory.Dispose();
			m_ShadeComponentFactory.Dispose();

			m_LightPresenters = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ILightsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				// Rebuild the light components.
				UnsubscribeLightComponents();

				IEnumerable<LightingProcessorControl> loads =
					Room == null
						? Enumerable.Empty<LightingProcessorControl>()
						: m_LightingProcessor.ContainsRoom(Room.Id)
							  ? m_LightingProcessor.GetLoadsForRoom(Room.Id).ToArray()
							  : Enumerable.Empty<LightingProcessorControl>();

				m_LightPresenters = m_LightComponentFactory.BuildChildren(loads);

				foreach (ILightComponentPresenter presenter in m_LightPresenters)
				{
					Subscribe(presenter);
					presenter.ShowView(true);
				}

				// Rebuild the shade components.
				UnsubscribeShadeComponents();

				IEnumerable<LightingProcessorControl> shades =
					Room == null
						? Enumerable.Empty<LightingProcessorControl>()
						: m_LightingProcessor.GetShadesForRoom(Room.Id)
						                     .Concat(m_LightingProcessor.GetShadeGroupsForRoom(Room.Id));

				foreach (IShadeComponentPresenter presenter in m_ShadeComponentFactory.BuildChildren(shades))
				{
					Subscribe(presenter);
					presenter.ShowView(true);
				}

				// Rebuild the presets list.
				m_Presets = Room == null
					            ? new LightingProcessorControl[0]
					            : m_LightingProcessor.GetPresetsForRoom(Room.Id).ToArray();
				view.SetLightPresetsLabels(m_Presets.Select(p => p.Name));
			}
			finally
			{
				m_RefreshSection.Leave();
			}

			RefreshPresetSelection();
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
		/// Generates the given number of light component views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<ILightComponentView> LightComponentViewFactory(ushort count)
		{
			return GetView().GetChildLightViews(ViewFactory, count);
		}

		/// <summary>
		/// Generates the given number of shade component views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IShadeComponentView> ShadeComponentViewFactory(ushort count)
		{
			return GetView().GetChildShadeViews(ViewFactory, count);
		}

		/// <summary>
		/// Refreshes the selection state of the preset buttons.
		/// </summary>
		private void RefreshPresetSelection()
		{
			m_RefreshSection.Enter();

			try
			{
				int? preset = Room == null ? null : m_LightingProcessor.GetPresetForRoom(Room.Id);
				int found = m_Presets.FindIndex(p => p.Id == preset);

				for (ushort index = 0; index < m_Presets.Length; index++)
					GetView().SetLightPresetSelected(index, index == found);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
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

			m_LightingProcessor.OnRoomControlsChanged += LightingProcessorOnRoomControlsChanged;
			m_LightingProcessor.OnRoomPresetChanged += LightingProcessorOnRoomPresetChanged;
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

			m_LightingProcessor.OnRoomControlsChanged -= LightingProcessorOnRoomControlsChanged;
			m_LightingProcessor.OnRoomPresetChanged -= LightingProcessorOnRoomPresetChanged;
		}

		/// <summary>
		/// Refresh the presenter when the controls change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="intEventArgs"></param>
		private void LightingProcessorOnRoomControlsChanged(object sender, IntEventArgs intEventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the room preset changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LightingProcessorOnRoomPresetChanged(object sender, RoomPresetChangeEventArgs args)
		{
			// Refresh here to avoid rebuilding the lists.
			if (IsViewVisible)
				RefreshPresetSelection();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ILightsView view)
		{
			base.Subscribe(view);

			view.OnLightListScrolling += ViewOnLightListScrolling;
			view.OnShadeListScrolling += ViewOnShadeListScrolling;
			view.OnPresetsListScrolling += ViewOnPresetsListScrolling;
			view.OnPresetButtonPressed += ViewOnPresetButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ILightsView view)
		{
			base.Unsubscribe(view);

			view.OnLightListScrolling -= ViewOnLightListScrolling;
			view.OnShadeListScrolling -= ViewOnShadeListScrolling;
			view.OnPresetsListScrolling -= ViewOnPresetsListScrolling;
			view.OnPresetButtonPressed -= ViewOnPresetButtonPressed;
		}

		/// <summary>
		/// Called when the user starts/stops scrolling the shade list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnShadeListScrolling(object sender, BoolEventArgs args)
		{
			SetListScrolling(args.Data);
		}

		/// <summary>
		/// Called when the user starts/stops scrolling the light list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnLightListScrolling(object sender, BoolEventArgs args)
		{
			SetListScrolling(args.Data);
		}

		/// <summary>
		/// Called when the user starts/stops scrolling the preset list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnPresetsListScrolling(object sender, BoolEventArgs args)
		{
			SetListScrolling(args.Data);
		}

		/// <summary>
		/// Called when the user presses a preset button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnPresetButtonPressed(object sender, UShortEventArgs args)
		{
			ResetVisibilityTimer();

			LightingProcessorControl preset = m_Presets[args.Data];
			m_LightingProcessor.SetPresetForRoom(preset);
		}

		/// <summary>
		/// While a list is scrolling we shouldn't hide the popup.
		/// On release we restart the visibility timer.
		/// </summary>
		/// <param name="data"></param>
		private void SetListScrolling(bool data)
		{
			if (data)
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

		#region LightComponent Callbacks

		/// <summary>
		/// Unsubscribe from all of the light component events.
		/// </summary>
		private void UnsubscribeLightComponents()
		{
			foreach (ILightComponentPresenter presenter in m_LightComponentFactory)
				Unsubscribe(presenter);
		}

		/// <summary>
		/// Subscribe to the light component events.
		/// </summary>
		/// <param name="lightComponent"></param>
		private void Subscribe(ILightComponentPresenter lightComponent)
		{
			lightComponent.OnButtonPressed += LightComponentOnButtonPressed;
			lightComponent.OnButtonReleased += LightComponentOnButtonReleased;
		}

		/// <summary>
		/// Unsubscribe from the light component events.
		/// </summary>
		/// <param name="lightComponent"></param>
		private void Unsubscribe(ILightComponentPresenter lightComponent)
		{
			lightComponent.OnButtonPressed -= LightComponentOnButtonPressed;
			lightComponent.OnButtonReleased -= LightComponentOnButtonReleased;
		}

		/// <summary>
		/// Called when the user presses the up button on a light.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LightComponentOnButtonPressed(object sender, EventArgs args)
		{
			StopVisibilityTimer();
		}

		/// <summary>
		/// Called when the user presses the down button on a light.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LightComponentOnButtonReleased(object sender, EventArgs args)
		{
			ResetVisibilityTimer();
		}

		#endregion

		#region ShadeComponent Callbacks

		/// <summary>
		/// Unsubscribe from all of the shade component events.
		/// </summary>
		private void UnsubscribeShadeComponents()
		{
			foreach (IShadeComponentPresenter presenter in m_ShadeComponentFactory)
				Unsubscribe(presenter);
		}

		/// <summary>
		/// Subscribe to the shade component events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(IShadeComponentPresenter presenter)
		{
			presenter.OnUpButtonPressed += PresenterOnUpButtonPressed;
			presenter.OnDownButtonPressed += PresenterOnDownButtonPressed;
			presenter.OnStopButtonPressed += PresenterOnStopButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the shade component events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(IShadeComponentPresenter presenter)
		{
			presenter.OnUpButtonPressed -= PresenterOnUpButtonPressed;
			presenter.OnDownButtonPressed -= PresenterOnDownButtonPressed;
			presenter.OnStopButtonPressed -= PresenterOnStopButtonPressed;
		}

		/// <summary>
		/// Called when a shade stop button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PresenterOnStopButtonPressed(object sender, EventArgs args)
		{
			ResetVisibilityTimer();
		}

		/// <summary>
		/// Called when a shade down button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PresenterOnDownButtonPressed(object sender, EventArgs args)
		{
			ResetVisibilityTimer();
		}

		/// <summary>
		/// Called when a shade up button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PresenterOnUpButtonPressed(object sender, EventArgs args)
		{
			ResetVisibilityTimer();
		}

		#endregion
	}
}
