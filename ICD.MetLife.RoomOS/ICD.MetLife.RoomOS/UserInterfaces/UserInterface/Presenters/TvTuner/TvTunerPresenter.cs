using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Settings.Core;
using ICD.Connect.Sources.TvTuner;
using ICD.Connect.TvPresets;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.TvTuner;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.TvTuner.NavSource;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.TvTuner
{
	public sealed class TvTunerPresenter : AbstractNavSourcePresenter<ITvTunerView>, ITvTunerPresenter
	{
		private readonly ChannelPresetPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		private ITvTuner m_Tuner;

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Watch TV"; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public TvTunerPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_ChildrenFactory = new ChannelPresetPresenterFactory(nav, ItemFactory);
			m_RefreshSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			UnsubscribeChildren();
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ITvTunerView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				m_Tuner = GetSourceControl().Parent as ITvTuner;

				UnsubscribeChildren();

				IEnumerable<Station> stations = Room == null ? Enumerable.Empty<Station>() : Room.TvPresets;

				foreach (IChannelPresetPresenter presenter in m_ChildrenFactory.BuildChildren(stations))
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

		#endregion

		#region Private Methods

		/// <summary>
		/// Generates the given number of views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<IChannelPresetView> ItemFactory(ushort count)
		{
			return GetView().GetChildCallViews(ViewFactory, count);
		}

		/// <summary>
		/// Unsubscribe from the child presenters.
		/// </summary>
		private void UnsubscribeChildren()
		{
			foreach (IChannelPresetPresenter presenter in m_ChildrenFactory)
				Unsubscribe(presenter);
		}

		#endregion

		#region Channel Presenter Callbacks

		/// <summary>
		/// Subscribe to the channel preset presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(IChannelPresetPresenter presenter)
		{
			presenter.OnPressed += PresenterOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the channel preset presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(IChannelPresetPresenter presenter)
		{
			presenter.OnPressed -= PresenterOnPressed;
		}

		/// <summary>
		/// Called when a channel preset is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void PresenterOnPressed(object sender, EventArgs eventArgs)
		{
			IChannelPresetPresenter presenter = sender as IChannelPresetPresenter;
			if (presenter == null)
				return;

			if (m_Tuner == null)
				return;

			string channel = presenter.Station.Channel;
			m_Tuner.SetChannel(channel);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ITvTunerView view)
		{
			base.Subscribe(view);

			view.OnChannelUpPressed += ViewOnChannelUpPressed;
			view.OnChannelDownPressed += ViewOnChannelDownPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ITvTunerView view)
		{
			base.Unsubscribe(view);

			view.OnChannelUpPressed -= ViewOnChannelUpPressed;
			view.OnChannelDownPressed -= ViewOnChannelDownPressed;
		}

		/// <summary>
		/// Called when the user presses the channel up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnChannelUpPressed(object sender, EventArgs eventArgs)
		{
			if (m_Tuner != null)
				m_Tuner.ChannelUp();
		}

		/// <summary>
		/// Called when the user presses the channel down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnChannelDownPressed(object sender, EventArgs eventArgs)
		{
			if (m_Tuner != null)
				m_Tuner.ChannelDown();
		}

		#endregion
	}
}
