using System;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils.Extensions;
using ICD.Connect.TvPresets;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.TvTuner
{
	public sealed class ChannelPresetPresenter : AbstractComponentPresenter<IChannelPresetView>, IChannelPresetPresenter
	{
		/// <summary>
		/// Raised when the user presses the channel preset.
		/// </summary>
		public event EventHandler OnPressed;

		private Station m_Station;

		/// <summary>
		/// Gets/sets the station.
		/// </summary>
		public Station Station
		{
			get { return m_Station; }
			set
			{
				if (value == m_Station)
					return;

				m_Station = value;

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
		public ChannelPresetPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IChannelPresetView view)
		{
			base.Refresh(view);

			view.SetImage(m_Station.Url ?? string.Empty);
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IChannelPresetView view)
		{
			base.Subscribe(view);

			view.OnPressed += ViewOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IChannelPresetView view)
		{
			base.Unsubscribe(view);

			view.OnPressed -= ViewOnPressed;
		}

		/// <summary>
		/// Called when the user presses the preset.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
