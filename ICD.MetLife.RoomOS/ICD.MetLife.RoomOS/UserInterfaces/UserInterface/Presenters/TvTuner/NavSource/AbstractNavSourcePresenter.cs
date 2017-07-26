using System;
using ICD.Common.Services.Logging;
using ICD.Connect.Rooms.Extensions;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner.NavSource;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner.NavSource;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.TvTuner.NavSource
{
	public abstract class AbstractNavSourcePresenter<TView> : AbstractMainPresenter<TView>, INavSourcePresenter
		where TView : class, INavSourceView
	{
		private MetlifeSource m_Source;

		#region Properties

		public MetlifeSource Source
		{
			get { return m_Source; }
			set
			{
				if (value == m_Source)
					return;

				m_Source = value;

				RefreshIfVisible();
			}
		}

		protected override string Title { get { return Source == null ? string.Empty : Source.GetNameOrDeviceName(Room); } }

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		protected AbstractNavSourcePresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(TView view)
		{
			base.Refresh(view);

			view.EnableStopButton(true);
		}

		protected IRouteSourceControl GetSourceControl()
		{
			return Source == null || Room == null
				       ? null
				       : Room.Devices.GetInstance(Source.Endpoint.Device).Controls.GetControl<IRouteSourceControl>();
		}

		#region View Callbacks

		protected override void Subscribe(TView view)
		{
			base.Subscribe(view);

			view.OnStopButtonPressed += ViewOnOnStopButtonPressed;
		}

		protected override void Unsubscribe(TView view)
		{
			base.Unsubscribe(view);

			view.OnStopButtonPressed -= ViewOnOnStopButtonPressed;
		}

		private void ViewOnOnStopButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				Logger.AddEntry(eSeverity.Error, "Unable to stop routing source - room is null");
			else
				Room.Routing.UnrouteSource(m_Source);

			ShowView(false);
		}

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			// Ignore hide
			if (!args.Data)
				return;

			if (Room == null)
				Logger.AddEntry(eSeverity.Error, "Unable to route source - room is null");
			else
				Room.Routing.Route(m_Source);
		}

		#endregion
	}
}
