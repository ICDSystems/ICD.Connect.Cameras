using System;
using ICD.Connect.Rooms;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Layout;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Layout;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Components.Video;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Layout
{
	public sealed class LayoutPresenter : AbstractMainPresenter<ILayoutView>, ILayoutPresenter
	{
		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Layout"; } }

		/// <summary>
		/// Gets the video component.
		/// </summary>
		public VideoComponent Video
		{
			get
			{
				CiscoCodec codec = Room.GetDevice<CiscoCodec>();
				return codec == null ? null : codec.Components.GetComponent<VideoComponent>();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public LayoutPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ILayoutView view)
		{
			base.Subscribe(view);

			view.OnAutoButtonPressed += ViewOnAutoButtonPressed;
			view.OnEqualButtonPressed += ViewOnEqualButtonPressed;
			view.OnProminentButtonPressed += ViewOnProminentButtonPressed;
			view.OnOverlayButtonPressed += ViewOnOverlayButtonPressed;
			view.OnSingleButtonPressed += ViewOnSingleButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ILayoutView view)
		{
			base.Unsubscribe(view);

			view.OnAutoButtonPressed -= ViewOnAutoButtonPressed;
			view.OnEqualButtonPressed -= ViewOnEqualButtonPressed;
			view.OnProminentButtonPressed -= ViewOnProminentButtonPressed;
			view.OnOverlayButtonPressed -= ViewOnOverlayButtonPressed;
			view.OnSingleButtonPressed -= ViewOnSingleButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the single layout button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSingleButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Video != null)
				Video.SetLayout(eLayoutTarget.Local, eLayoutFamily.Single);
		}

		/// <summary>
		/// Called when the user presses the overlay layout button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnOverlayButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Video != null)
				Video.SetLayout(eLayoutTarget.Local, eLayoutFamily.Overlay);
		}

		/// <summary>
		/// Called when the user presses the prominent layout button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnProminentButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Video != null)
				Video.SetLayout(eLayoutTarget.Local, eLayoutFamily.Prominent);
		}

		/// <summary>
		/// Called when the user presses the equal layout button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnEqualButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Video != null)
				Video.SetLayout(eLayoutTarget.Local, eLayoutFamily.Equal);
		}

		/// <summary>
		/// Called when the user presses the auto layout button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnAutoButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Video != null)
				Video.SetLayout(eLayoutTarget.Local, eLayoutFamily.Auto);
		}

		#endregion
	}
}
