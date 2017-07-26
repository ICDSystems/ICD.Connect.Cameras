using System;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.Connect.Conferencing.Conferences;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.MainNav.Components
{
	public sealed class MainNavEndCallComponentPresenter : AbstractMainNavComponentPresenter,
	                                                       IMainNavEndCallComponentPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public MainNavEndCallComponentPresenter(int room, INavigationController nav, IViewFactory views,
		                                        ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Gets the label text for the component.
		/// </summary>
		/// <returns></returns>
		protected override string GetLabel()
		{
			IConference conference = Room == null ? null : Room.ConferenceManager.ActiveConference;
			if (conference == null)
				return string.Empty;

			string call = conference.SourcesCount > 1 ? "Calls" : "Call";
			return string.Format("End {0}", call);
		}

		/// <summary>
		/// Gets the current icon state for the component.
		/// </summary>
		/// <returns></returns>
		protected override eIconState GetIconState()
		{
			return eIconState.Default;
		}

		/// <summary>
		/// Gets the icon for the component.
		/// </summary>
		/// <returns></returns>
		protected override IIcon GetIcon()
		{
			return GetView().GetIcon(eMainNavIcon.EndCall);
		}

		/// <summary>
		/// Called when the component is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			IConference conference = Room == null ? null : Room.ConferenceManager.ActiveConference;
			if (conference != null)
				conference.Hangup();
		}
	}
}
