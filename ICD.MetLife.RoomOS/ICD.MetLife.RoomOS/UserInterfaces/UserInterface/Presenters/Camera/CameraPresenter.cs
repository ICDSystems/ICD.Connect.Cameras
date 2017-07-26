using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Rooms;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Camera;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Camera;
using ICD.Connect.Conferencing.Cameras;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Components.Cameras;
using ICD.Connect.Conferencing.Cisco.Components.Video;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Camera
{
	public sealed class CameraPresenter : AbstractMainPresenter<ICameraView>, ICameraPresenter
	{
		private ICamera m_CameraSelection;

		private VideoComponent m_Video;
		private NearCamerasComponent m_NearCameras;
		private CameraNamePair[] m_Cameras;

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Cameras"; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public CameraPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ICameraView view)
		{
			base.Refresh(view);

			// Self-view buttons
			view.SetSelfViewActive(m_Video.SelfViewEnabled);
			view.SetSelfViewFullscreenActive(m_Video.SelfViewFullScreenEnabled);
			view.SetSelfViewFullscreenVisible(m_Video.SelfViewEnabled);

			// Cameras
			m_Cameras = GetCameras().ToArray();

			// If we don't have a camera selected, select the active local camera
			if (m_CameraSelection == null)
			{
				int source = m_Video.MainVideoSource;
				m_CameraSelection = m_Cameras.Select(c => c.Camera)
				                             .OfType<NearCamera>()
				                             .FirstOrDefault(c => c.CameraId == source);
			}

			int selected = m_Cameras.FindIndex(c => c.Camera == m_CameraSelection);

			view.SetCameraLabels(m_Cameras.Select(c => c.Name));
			for (ushort index = 0; index < m_Cameras.Length; index++)
				view.SetCameraSelected(index, index == selected);
		}

		#region Private Methods

		/// <summary>
		/// Moves the current selected camera.
		/// </summary>
		/// <param name="action"></param>
		private void CameraMove(eCameraAction action)
		{
			if (m_CameraSelection != null)
				m_CameraSelection.Move(action);
		}

		/// <summary>
		/// Stops the current selected camera.
		/// </summary>
		private void CameraStop()
		{
			if (m_CameraSelection != null)
				m_CameraSelection.Stop();
		}

		/// <summary>
		/// Sends the given camera to the conference (it shows in selfview and remote contacts can see it).
		/// </summary>
		/// <param name="camera"></param>
		private void SetCameraSource(NearCamera camera)
		{
			if (m_Video != null)
				m_Video.SetMainVideoSource(camera.CameraId);
		}

		/// <summary>
		/// Gets all of the near and far cameras.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<CameraNamePair> GetCameras()
		{
			foreach (NearCamera camera in m_NearCameras.GetConnectedCameras())
			{
				string name = string.Format("Camera {0}", camera.CameraId);
				yield return new CameraNamePair(name, camera);
			}

			if (Room == null)
				yield break;

			if (!Room.ConferenceManager.GetIsActiveConferenceOnline())
				yield break;

			foreach (IConferenceSource source in Room.ConferenceManager.ActiveConference.GetOnlineSources().Where(s => s.Camera != null))
				yield return new CameraNamePair(source.Name, source.Camera);
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

			CiscoCodec codec = Room.GetDevice<CiscoCodec>();
			if (codec != null)
			{
				m_Video = codec.Components.GetComponent<VideoComponent>();
				m_NearCameras = codec.Components.GetComponent<NearCamerasComponent>();

				m_NearCameras.OnCamerasChanged += NearCamerasOnCamerasChanged;
				m_NearCameras.OnPresetsChanged += NearCamerasOnPresetsChanged;
				m_Video.OnSelfViewEnabledChanged += VideoOnSelfViewEnabledChanged;
				m_Video.OnSelfViewFullScreenEnabledChanged += VideoOnSelfViewFullScreenEnabledChanged;
			}

			room.ConferenceManager.OnRecentSourceAdded += DialingPlanOnRecentSourceAdded;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			if (m_NearCameras != null)
			{
				m_NearCameras.OnCamerasChanged -= NearCamerasOnCamerasChanged;
				m_NearCameras.OnPresetsChanged -= NearCamerasOnPresetsChanged;
			}

			if (m_Video != null)
			{
				m_Video.OnSelfViewEnabledChanged -= VideoOnSelfViewEnabledChanged;
				m_Video.OnSelfViewFullScreenEnabledChanged -= VideoOnSelfViewFullScreenEnabledChanged;
			}

			if (room == null)
				return;

			room.ConferenceManager.OnRecentSourceAdded -= DialingPlanOnRecentSourceAdded;
		}

		/// <summary>
		/// Called when the self-view fullscreen state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void VideoOnSelfViewFullScreenEnabledChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the self-view enabled state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void VideoOnSelfViewEnabledChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when a source is added.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="conferenceSourceEventArgs"></param>
		private void DialingPlanOnRecentSourceAdded(object sender, ConferenceSourceEventArgs conferenceSourceEventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when a camera is added to the system.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void NearCamerasOnCamerasChanged(object sender, EventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the camera presets change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void NearCamerasOnPresetsChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ICameraView view)
		{
			base.Subscribe(view);

			view.OnCameraSelected += ViewOnCameraSelected;
			view.OnCameraMoveUpButtonPressed += ViewOnCameraMoveUpButtonPressed;
			view.OnCameraMoveLeftButtonPressed += ViewOnCameraMoveLeftButtonPressed;
			view.OnCameraMoveRightButtonPressed += ViewOnCameraMoveRightButtonPressed;
			view.OnCameraMoveDownButtonPressed += ViewOnCameraMoveDownButtonPressed;
			view.OnCameraZoomInButtonPressed += ViewOnCameraZoomInButtonPressed;
			view.OnCameraZoomOutButtonPressed += ViewOnCameraZoomOutButtonPressed;
			view.OnCameraButtonReleased += ViewOnCameraButtonReleased;
			view.OnSelfViewButtonPressed += ViewOnSelfViewButtonPressed;
			view.OnSelfViewFullscreenButtonPressed += ViewOnSelfViewFullscreenButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ICameraView view)
		{
			base.Unsubscribe(view);

			view.OnCameraSelected -= ViewOnCameraSelected;
			view.OnCameraMoveUpButtonPressed -= ViewOnCameraMoveUpButtonPressed;
			view.OnCameraMoveLeftButtonPressed -= ViewOnCameraMoveLeftButtonPressed;
			view.OnCameraMoveRightButtonPressed -= ViewOnCameraMoveRightButtonPressed;
			view.OnCameraMoveDownButtonPressed -= ViewOnCameraMoveDownButtonPressed;
			view.OnCameraZoomInButtonPressed -= ViewOnCameraZoomInButtonPressed;
			view.OnCameraZoomOutButtonPressed -= ViewOnCameraZoomOutButtonPressed;
			view.OnCameraButtonReleased -= ViewOnCameraButtonReleased;
			view.OnSelfViewButtonPressed -= ViewOnSelfViewButtonPressed;
			view.OnSelfViewFullscreenButtonPressed -= ViewOnSelfViewFullscreenButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the self-view fullscreen button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSelfViewFullscreenButtonPressed(object sender, EventArgs eventArgs)
		{
			bool fullscreen = m_Video.SelfViewFullScreenEnabled;
			m_Video.SetSelfViewFullScreen(!fullscreen);
		}

		/// <summary>
		/// Called when the user presses the self-view button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSelfViewButtonPressed(object sender, EventArgs eventArgs)
		{
			bool selfView = m_Video.SelfViewEnabled;
			m_Video.SetSelfViewEnabled(!selfView);
		}

		/// <summary>
		/// Called when a camera button is released.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCameraButtonReleased(object sender, EventArgs eventArgs)
		{
			CameraStop();
		}

		/// <summary>
		/// Called when the user presses the zoom out button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCameraZoomOutButtonPressed(object sender, EventArgs eventArgs)
		{
			CameraMove(eCameraAction.ZoomOut);
		}

		/// <summary>
		/// Called when the user presses the zoom in button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCameraZoomInButtonPressed(object sender, EventArgs eventArgs)
		{
			CameraMove(eCameraAction.ZoomIn);
		}

		/// <summary>
		/// Called when the user presses the down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCameraMoveDownButtonPressed(object sender, EventArgs eventArgs)
		{
			CameraMove(eCameraAction.Down);
		}

		/// <summary>
		/// Called when the user presses the right button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCameraMoveRightButtonPressed(object sender, EventArgs eventArgs)
		{
			CameraMove(eCameraAction.Right);
		}

		/// <summary>
		/// Called when the user presses the left button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCameraMoveLeftButtonPressed(object sender, EventArgs eventArgs)
		{
			CameraMove(eCameraAction.Left);
		}

		/// <summary>
		/// Called when the user presses the up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCameraMoveUpButtonPressed(object sender, EventArgs eventArgs)
		{
			CameraMove(eCameraAction.Up);
		}

		/// <summary>
		/// Called when the user presses a camera selection button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCameraSelected(object sender, UShortEventArgs eventArgs)
		{
			m_CameraSelection = m_Cameras.ElementAt(eventArgs.Data).Camera;
			
			if (m_CameraSelection is NearCamera)
				SetCameraSource(m_CameraSelection as NearCamera);

			RefreshIfVisible();
		}

		#endregion

		private struct CameraNamePair
		{
			private readonly string m_Name;
			private readonly ICamera m_Camera;

			public string Name { get { return m_Name; } }
			public ICamera Camera { get { return m_Camera; } }

			public CameraNamePair(string name, ICamera camera)
			{
				m_Name = name;
				m_Camera = camera;
			}
		}
	}
}
