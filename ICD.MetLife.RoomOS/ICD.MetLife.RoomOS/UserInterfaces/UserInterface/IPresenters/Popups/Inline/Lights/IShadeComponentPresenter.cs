using System;
using ICD.Connect.Lighting;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Lights
{
	public interface IShadeComponentPresenter : IPresenter<IShadeComponentView>
	{
		/// <summary>
		/// Raised when the user presses the up button.
		/// </summary>
		event EventHandler OnUpButtonPressed;

		/// <summary>
		/// Raised when the user presses the down button.
		/// </summary>
		event EventHandler OnDownButtonPressed;

		/// <summary>
		/// Raised when the user presses the stop button.
		/// </summary>
		event EventHandler OnStopButtonPressed;

		/// <summary>
		/// Gets/sets the lighting control.
		/// </summary>
		LightingProcessorControl Control { get; set; }
	}
}
