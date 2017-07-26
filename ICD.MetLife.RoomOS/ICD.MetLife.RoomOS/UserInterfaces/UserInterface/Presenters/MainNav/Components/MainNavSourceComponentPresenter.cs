using System;
using ICD.Common.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Rooms.Extensions;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Connect.Sources.TvTuner;
using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner.NavSource;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.MainNav.Components
{
	public sealed class MainNavSourceComponentPresenter : AbstractMainNavComponentPresenter,
	                                                      IMainNavSourceComponentPresenter
	{
		private INavSourcePresenter m_CachedMenu;

		/// <summary>
		/// Set the device source to switch to for this nav component
		/// </summary>
		/// <value></value>
		public MetlifeSource Source { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public MainNavSourceComponentPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			if (m_CachedMenu != null)
				Unsubscribe(m_CachedMenu);

			base.Dispose();
		}

		#region Protected Methods

		/// <summary>
		/// Gets the label of the nav component
		/// </summary>
		/// <returns></returns>
		protected override string GetLabel()
		{
			if (Source == null)
				return string.Empty;

			string verb = Source.SourceType.GetSourceVerb();
			string name = Source.GetNameOrDeviceName(Room);

			return string.Format("{0} {1}", verb, name);
		}

		/// <summary>
		/// Gets the icon of the nav component
		/// </summary>
		/// <returns></returns>
		protected override IIcon GetIcon()
		{
			return GetView().GetIcon(Source.SourceType);
		}

		/// <summary>
		/// Gets the state of the nav component icon
		/// </summary>
		/// <returns></returns>
		protected override eIconState GetIconState()
		{
			return m_CachedMenu != null && m_CachedMenu.IsViewVisible ? eIconState.Active : eIconState.Default;
		}

		/// <summary>
		/// Called when the source icon is pressed. Shows the menu corresponding to that device
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			if (m_CachedMenu == null)
			{
				IDevice device = Room == null ? null : Room.Devices.GetInstance(Source.Endpoint.Device);

				if (device == null)
					m_CachedMenu = null;
				else if (device is ITvTuner)
					m_CachedMenu = Navigation.LazyLoadPresenter<ITvTunerPresenter>();
				else
					m_CachedMenu = Navigation.LazyLoadPresenter<IGenericNavSourcePresenter>();

				Subscribe(m_CachedMenu);
			}

			if (m_CachedMenu == null)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to show source menu - device is null");
				return;
			}

			m_CachedMenu.Source = Source;
			m_CachedMenu.ShowView(!m_CachedMenu.IsViewVisible);
		}

		#endregion

		#region Navigation Callbacks

		/// <summary>
		/// Subscribe to the menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Subscribe(IPresenter menu)
		{
			menu.OnViewVisibilityChanged += MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the menu events.
		/// </summary>
		/// <param name="menu"></param>
		private void Unsubscribe(IPresenter menu)
		{
			menu.OnViewVisibilityChanged -= MenuOnViewVisibilityChanged;
		}

		/// <summary>
		/// Called when the menu visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void MenuOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
