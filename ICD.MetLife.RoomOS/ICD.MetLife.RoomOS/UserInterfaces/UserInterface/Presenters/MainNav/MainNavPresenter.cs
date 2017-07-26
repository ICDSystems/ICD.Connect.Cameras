using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Rooms;
using ICD.Connect.Scheduling.Asure;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.Endpoints.Sources;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.MainNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.MainNav;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Components.Presentation;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.MainNav
{
	public sealed class MainNavPresenter : AbstractPresenter<IMainNavView>, IMainNavPresenter
	{
		/// <summary>
		/// Ordered list of the components contained in the nav bar.
		/// </summary>
		private static readonly Type[] s_StaticChildrenTypes =
		{
			typeof(IMainNavMuteComponentPresenter),
			typeof(IMainNavShareComponentPresenter),
			typeof(IMainNavCameraComponentPresenter),
			typeof(IMainNavLayoutComponentPresenter),
			typeof(IMainNavTouchtonesComponentPresenter),
			typeof(IMainNavDialComponentPresenter),
			typeof(IMainNavContactsComponentPresenter),
			typeof(IMainNavMeetingsComponentPresenter),
			typeof(IMainNavAddCallComponentPresenter),
			typeof(IMainNavHoldCallComponentPresenter),
			typeof(IMainNavEndCallComponentPresenter)
		};

		/// <summary>
		/// Don't show these items if the room has no codec.
		/// </summary>
		private static readonly Type[] s_CodecTypes =
		{
			typeof(IMainNavMuteComponentPresenter),
			typeof(IMainNavCameraComponentPresenter),
			typeof(IMainNavLayoutComponentPresenter),
			typeof(IMainNavTouchtonesComponentPresenter),
			typeof(IMainNavDialComponentPresenter),
			typeof(IMainNavContactsComponentPresenter),
			typeof(IMainNavMeetingsComponentPresenter),
			typeof(IMainNavAddCallComponentPresenter),
			typeof(IMainNavHoldCallComponentPresenter),
			typeof(IMainNavEndCallComponentPresenter)
		};

		/// <summary>
		/// Components that are visible while not in a conference.
		/// </summary>
		private static readonly Type[] s_OutOfCallChildren =
		{
			typeof(IMainNavShareComponentPresenter),
			typeof(IMainNavSourceComponentPresenter),
			typeof(IMainNavDialComponentPresenter),
			typeof(IMainNavContactsComponentPresenter),
			typeof(IMainNavMeetingsComponentPresenter)
		};

		/// <summary>
		/// Components that are visible while in a conference.
		/// </summary>
		private static readonly Type[] s_InCallChildren =
		{
			typeof(IMainNavMuteComponentPresenter),
			typeof(IMainNavShareComponentPresenter),
			typeof(IMainNavCameraComponentPresenter),
			typeof(IMainNavLayoutComponentPresenter),
			typeof(IMainNavTouchtonesComponentPresenter),
			typeof(IMainNavAddCallComponentPresenter),
			typeof(IMainNavHoldCallComponentPresenter),
			typeof(IMainNavEndCallComponentPresenter)
		};

		private readonly List<IMainNavComponentPresenter> m_Children;
		private readonly SafeCriticalSection m_ChildrenSection;

		private PresentationComponent m_Presentation;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public MainNavPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_Children = new List<IMainNavComponentPresenter>();
			m_ChildrenSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			DisposeChildren();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IMainNavView view)
		{
			base.Refresh(view);

			m_ChildrenSection.Enter();

			try
			{
				if (m_Children.Count == 0)
					BuildChildComponents();
			}
			finally
			{
				m_ChildrenSection.Leave();
			}

			Type[] visibleTypes = GetVisibleTypes().ToArray();
			foreach (IMainNavComponentPresenter child in m_Children)
				child.ShowView(visibleTypes.Any(t => t.IsInstanceOfType(child)));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Builds the nav bar components.
		/// </summary>
		private void BuildChildComponents()
		{
			// Build the presenters and views
			IMainNavComponentPresenter[] presenters = MainNavComponentFactory();
			IMainNavComponentView[] views = GetView().GetChildComponentViews(ViewFactory, (ushort)presenters.Length).ToArray();

			// Bind the views
			for (int index = 0; index < views.Length; index++)
			{
				IMainNavComponentPresenter presenter = presenters[index];
				presenter.SetView(views[index]);
			}

			m_Children.AddRange(presenters);
		}

		/// <summary>
		/// Builds the main nav component presenters.
		/// </summary>
		/// <returns></returns>
		private IMainNavComponentPresenter[] MainNavComponentFactory()
		{
			List<IMainNavComponentPresenter> output = new List<IMainNavComponentPresenter>(s_StaticChildrenTypes.Length);
			output.AddRange(s_StaticChildrenTypes.Select(type => Navigation.GetNewPresenter(type) as IMainNavComponentPresenter));

			// Insert nav sources after the share component
			int index = output.FindIndex(p => p as IMainNavShareComponentPresenter != null);
			output.InsertRange(index + 1, MainNavSourceComponentFactory());

			return output.ToArray();
		}

		/// <summary>
		/// Builds the presenters for the main nav source devices.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IMainNavComponentPresenter> MainNavSourceComponentFactory()
		{
			if (Room == null)
				yield break;

			foreach (MetlifeSource source in Room.Routing.GetSources(eSourceFlags.MainNav))
			{
				IMainNavSourceComponentPresenter presenter =
					Navigation.GetNewPresenter<IMainNavSourceComponentPresenter>();

				presenter.Source = source;
				yield return presenter;
			}
		}

		/// <summary>
		/// Returns the component types that should be visible.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<Type> GetVisibleTypes()
		{
			bool isInCall = Room != null && Room.ConferenceManager.IsInCall;
			Type[] visibleTypes = isInCall ? s_InCallChildren : s_OutOfCallChildren;

			// Don't show codec buttons if there is no codec.
			// TODO - should we check the dialling plan instead?
			if (Room != null && Room.GetDevice<CiscoCodec>() == null)
				visibleTypes = visibleTypes.Except(s_CodecTypes).ToArray();

			// Don't show meetings if there is no Asure service
			if (Room != null && Room.GetDevice<AsureDevice>() == null)
				visibleTypes = visibleTypes.Except(typeof(IMainNavMeetingsComponentPresenter)).ToArray();

			// Only show the Layout button if content is being shared through the codec
			if (m_Presentation == null || m_Presentation.PresentationMode == ePresentationMode.Off)
				visibleTypes = visibleTypes.Except(typeof(IMainNavLayoutComponentPresenter)).ToArray();

			return visibleTypes;
		}

		/// <summary>
		/// Disposes the child presenters.
		/// </summary>
		private void DisposeChildren()
		{
			foreach (IMainNavComponentPresenter presenter in m_Children)
				presenter.Dispose();
			m_Children.Clear();
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

			if (room == null)
				return;

			room.ConferenceManager.OnInCallChanged += ConferenceManagerOnInCallChanged;

			CiscoCodec codec = room.GetDevice<CiscoCodec>();
			if (codec != null)
			{
				m_Presentation = codec.Components.GetComponent<PresentationComponent>();
				m_Presentation.OnPresentationModeChanged += PresentationOnPresentationsChanged;
			}
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (m_Presentation != null)
				m_Presentation.OnPresentationModeChanged -= PresentationOnPresentationsChanged;

			if (room == null)
				return;

			room.ConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;
		}

		/// <summary>
		/// Called when we enter or leave a conference.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ConferenceManagerOnInCallChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible(false);
		}

		/// <summary>
		/// Called when we enter/leave a presentation.
		/// 
		/// We only show the layout button if we're in a presentation.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void PresentationOnPresentationsChanged(object sender, PresentationModeEventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
