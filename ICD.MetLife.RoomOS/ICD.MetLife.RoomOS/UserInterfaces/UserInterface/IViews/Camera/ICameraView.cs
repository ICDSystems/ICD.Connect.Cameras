using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Camera
{
	public interface ICameraView : IView
	{
		#region Events

		/// <summary>
		/// Raised when the user selects a camera to control.
		/// </summary>
		event EventHandler<UShortEventArgs> OnCameraSelected;

		/// <summary>
		/// Raised when the up button is pressed.
		/// </summary>
		event EventHandler OnCameraMoveUpButtonPressed;

		/// <summary>
		/// Raised when the left button is pressed.
		/// </summary>
		event EventHandler OnCameraMoveLeftButtonPressed;

		/// <summary>
		/// Raised when the right button is pressed.
		/// </summary>
		event EventHandler OnCameraMoveRightButtonPressed;

		/// <summary>
		/// Raised when the down button is pressed.
		/// </summary>
		event EventHandler OnCameraMoveDownButtonPressed;

		/// <summary>
		/// Raised when the zoom in button is pressed.
		/// </summary>
		event EventHandler OnCameraZoomInButtonPressed;

		/// <summary>
		/// Raised when the zoom out button is pressed.
		/// </summary>
		event EventHandler OnCameraZoomOutButtonPressed;

		/// <summary>
		/// Raised when any camera button is released (i.e. stop the camera).
		/// </summary>
		event EventHandler OnCameraButtonReleased;

		/// <summary>
		/// Raises when the user presses the self view button.
		/// </summary>
		event EventHandler OnSelfViewButtonPressed;

		/// <summary>
		/// Raises when the user presses the self view fullscreen button.
		/// </summary>
		event EventHandler OnSelfViewFullscreenButtonPressed;

		#endregion

		#region Methods

		/// <summary>
		/// Sets the camera labels.
		/// </summary>
		/// <param name="names"></param>
		void SetCameraLabels(IEnumerable<string> names);

		/// <summary>
		/// Sets the selection state of the camera.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetCameraSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the active state of the self-view button.
		/// </summary>
		/// <param name="state"></param>
		void SetSelfViewActive(bool state);

		/// <summary>
		/// Sets the active state of the self-view fullscreen button.
		/// </summary>
		/// <param name="state"></param>
		void SetSelfViewFullscreenActive(bool state);

		/// <summary>
		/// Sets the visibility of the self-view fullscreen button.
		/// </summary>
		/// <param name="visible"></param>
		void SetSelfViewFullscreenVisible(bool visible);

		#endregion
	}
}
