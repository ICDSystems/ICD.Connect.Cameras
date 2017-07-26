using System;
using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TvTuner.NavSource;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TvTuner
{
	public sealed partial class TvTunerView : AbstractNavSourceView, ITvTunerView
	{
		public event EventHandler OnChannelUpPressed;
		public event EventHandler OnChannelDownPressed;

		private readonly List<IChannelPresetView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public TvTunerView(ISigInputOutput panel)
			: base(panel)
		{
			m_ChildList = new List<IChannelPresetView>();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnChannelDownPressed = null;
			OnChannelDownPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IChannelPresetView> GetChildCallViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ChannelList, m_ChildList, count);
		}

		/// <summary>
		/// Sets the enabled state of the stop watching tv button.
		/// </summary>
		/// <param name="enabled"></param>
		public override void EnableStopButton(bool enabled)
		{
			m_StopWatchingTvButton.Enable(enabled);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ChannelUpButton.OnPressed += ChannelUpButtonOnPressed;
			m_ChannelDownButton.OnPressed += ChannelDownButtonOnPressed;
			m_StopWatchingTvButton.OnPressed += StopWatchingTvButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ChannelUpButton.OnPressed -= ChannelUpButtonOnPressed;
			m_ChannelDownButton.OnPressed -= ChannelDownButtonOnPressed;
			m_StopWatchingTvButton.OnPressed -= StopWatchingTvButtonOnPressed;
		}

		/// <summary>
		/// Called when the channel up button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ChannelUpButtonOnPressed(object sender, EventArgs args)
		{
			OnChannelUpPressed.Raise(this);
		}

		/// <summary>
		/// Called when the channel down button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ChannelDownButtonOnPressed(object sender, EventArgs args)
		{
			OnChannelDownPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the stop watching tv button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void StopWatchingTvButtonOnPressed(object sender, EventArgs args)
		{
			RaiseOnStopButtonPressed();
		}

		#endregion
	}
}
