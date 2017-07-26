using System;
using ICD.Connect.Lighting;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Inline.Lights
{
	public interface ILightComponentPresenter : IPresenter<ILightComponentView>
	{
		/// <summary>
		/// Raised when the user presses a button.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// Raised when the user releases a button.
		/// </summary>
		event EventHandler OnButtonReleased;

		/// <summary>
		/// Gets/sets the lighting control.
		/// </summary>
		LightingProcessorControl Control { get; set; }
	}
}
