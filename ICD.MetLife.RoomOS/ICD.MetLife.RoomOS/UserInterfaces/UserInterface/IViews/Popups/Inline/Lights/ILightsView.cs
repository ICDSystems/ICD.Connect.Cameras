using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights
{
	public interface ILightsView : IView
	{
		/// <summary>
		/// Raises true when the light list is scrolling.
		/// </summary>
		event EventHandler<BoolEventArgs> OnLightListScrolling;

		/// <summary>
		/// Raises true when the shade list is scrolling.
		/// </summary>
		event EventHandler<BoolEventArgs> OnShadeListScrolling;

		/// <summary>
		/// Raises true when the presets list is scrolling.
		/// </summary>
		event EventHandler<BoolEventArgs> OnPresetsListScrolling;

		/// <summary>
		/// Raised when the user presses a preset button.
		/// </summary>
		event EventHandler<UShortEventArgs> OnPresetButtonPressed;

		/// <summary>
		/// Sets the labels for the light preset buttons.
		/// </summary>
		/// <param name="labels"></param>
		void SetLightPresetsLabels(IEnumerable<string> labels);

		/// <summary>
		/// Sets the selected state of the preset button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetLightPresetSelected(ushort index, bool selected);

		/// <summary>
		/// Returns child views for light items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<ILightComponentView> GetChildLightViews(IViewFactory factory, ushort count);

		/// <summary>
		/// Returns child views for shade items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IShadeComponentView> GetChildShadeViews(IViewFactory factory, ushort count);
	}
}
