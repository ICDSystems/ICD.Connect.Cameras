using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Connect.UI.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Camera;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Camera
{
	public sealed partial class CameraView : AbstractView, ICameraView
	{
		public event EventHandler<UShortEventArgs> OnCameraSelected;
		public event EventHandler OnCameraMoveUpButtonPressed;
		public event EventHandler OnCameraMoveLeftButtonPressed;
		public event EventHandler OnCameraMoveRightButtonPressed;
		public event EventHandler OnCameraMoveDownButtonPressed;
		public event EventHandler OnCameraZoomInButtonPressed;
		public event EventHandler OnCameraZoomOutButtonPressed;
		public event EventHandler OnCameraButtonReleased;
		public event EventHandler OnSelfViewButtonPressed;
		public event EventHandler OnSelfViewFullscreenButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public CameraView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCameraSelected = null;
			OnCameraMoveUpButtonPressed = null;
			OnCameraMoveLeftButtonPressed = null;
			OnCameraMoveRightButtonPressed = null;
			OnCameraMoveDownButtonPressed = null;
			OnCameraZoomInButtonPressed = null;
			OnCameraZoomOutButtonPressed = null;
			OnCameraButtonReleased = null;
			OnSelfViewButtonPressed = null;
			OnSelfViewFullscreenButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the camera labels.
		/// </summary>
		/// <param name="names"></param>
		public void SetCameraLabels(IEnumerable<string> names)
		{
			string[] namesArray = names.Take(m_CameraButtonList.MaxSize).ToArray();

			m_CameraButtonList.SetNumberOfItems((ushort)namesArray.Length);

			for (ushort index = 0; index < (ushort)namesArray.Length; index++)
			{
				m_CameraButtonList.SetItemVisible(index, true);
				m_CameraButtonList.SetItemLabel(index, namesArray[index]);
			}
		}

		/// <summary>
		/// Sets the selection state of the camera.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetCameraSelected(ushort index, bool selected)
		{
			if (index < m_CameraButtonList.MaxSize)
				m_CameraButtonList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the active state of the self-view button.
		/// </summary>
		/// <param name="state"></param>
		public void SetSelfViewActive(bool state)
		{
			m_SelfViewButton.SetSelected(state);
		}

		/// <summary>
		/// Sets the active state of the self-view fullscreen button.
		/// </summary>
		/// <param name="state"></param>
		public void SetSelfViewFullscreenActive(bool state)
		{
			m_SelfViewFullscreenButton.SetSelected(state);
		}

		/// <summary>
		/// Sets the visibility of the self-view fullscreen button.
		/// </summary>
		/// <param name="visible"></param>
		public void SetSelfViewFullscreenVisible(bool visible)
		{
			m_SelfViewFullscreenButton.Show(visible);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CameraButtonList.OnButtonClicked += CameraButtonListOnButtonClicked;
			m_DPad.OnButtonPressed += DPadOnButtonPressed;
			m_DPad.OnButtonReleased += DPadOnButtonReleased;
			m_ZoomInButton.OnPressed += ZoomInButtonOnPressed;
			m_ZoomInButton.OnReleased += ZoomInButtonOnReleased;
			m_ZoomOutButton.OnPressed += ZoomOutButtonOnPressed;
			m_ZoomOutButton.OnReleased += ZoomOutButtonOnReleased;
			m_SelfViewButton.OnPressed += SelfViewButtonOnPressed;
			m_SelfViewFullscreenButton.OnPressed += SelfViewFullscreenButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CameraButtonList.OnButtonClicked -= CameraButtonListOnButtonClicked;
			m_DPad.OnButtonPressed -= DPadOnButtonPressed;
			m_DPad.OnButtonReleased -= DPadOnButtonReleased;
			m_ZoomInButton.OnPressed -= ZoomInButtonOnPressed;
			m_ZoomInButton.OnReleased -= ZoomInButtonOnReleased;
			m_ZoomOutButton.OnPressed -= ZoomOutButtonOnPressed;
			m_ZoomOutButton.OnReleased -= ZoomOutButtonOnReleased;
			m_SelfViewButton.OnPressed -= SelfViewButtonOnPressed;
			m_SelfViewFullscreenButton.OnPressed -= SelfViewFullscreenButtonOnPressed;
		}

		/// <summary>
		/// Called when the user releases the zoom out button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ZoomOutButtonOnReleased(object sender, EventArgs args)
		{
			OnCameraButtonReleased.Raise(this);
		}

		/// <summary>
		/// Called when the user releases the zoom in button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ZoomInButtonOnReleased(object sender, EventArgs args)
		{
			OnCameraButtonReleased.Raise(this);
		}

		/// <summary>
		/// Called when the user releases a dpad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="dPadEventArgs"></param>
		private void DPadOnButtonReleased(object sender, DPadEventArgs dPadEventArgs)
		{
			OnCameraButtonReleased.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the fullscreen button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SelfViewFullscreenButtonOnPressed(object sender, EventArgs args)
		{
			OnSelfViewFullscreenButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the self-view button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SelfViewButtonOnPressed(object sender, EventArgs args)
		{
			OnSelfViewButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the zoom out button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ZoomOutButtonOnPressed(object sender, EventArgs args)
		{
			OnCameraZoomOutButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the zoom in button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ZoomInButtonOnPressed(object sender, EventArgs args)
		{
			OnCameraZoomInButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses a dpad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="dPadEventArgs"></param>
		private void DPadOnButtonPressed(object sender, DPadEventArgs dPadEventArgs)
		{
			switch (dPadEventArgs.Data)
			{
				case DPadEventArgs.eDirection.Up:
					OnCameraMoveUpButtonPressed.Raise(this);
					break;
				case DPadEventArgs.eDirection.Down:
					OnCameraMoveDownButtonPressed.Raise(this);
					break;
				case DPadEventArgs.eDirection.Left:
					OnCameraMoveLeftButtonPressed.Raise(this);
					break;
				case DPadEventArgs.eDirection.Right:
					OnCameraMoveRightButtonPressed.Raise(this);
					break;
				case DPadEventArgs.eDirection.Center:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Called when the user presses a camera button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CameraButtonListOnButtonClicked(object sender, UShortEventArgs args)
		{
			OnCameraSelected.Raise(this, new UShortEventArgs(args.Data));
		}

		#endregion
	}
}
