using System;
using ICD.MetLife.RoomOS.Endpoints.Sources;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.MainNav
{
	/// <summary>
	/// Represents a single item on the main nav view.
	/// </summary>
	public interface IMainNavComponentView : IView
	{
		/// <summary>
		/// Raised when the user presses the main nav component.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Sets the label text for the component.
		/// </summary>
		/// <param name="text"></param>
		void SetLabelText(string text);

		/// <summary>
		/// Sets the icon for the component.
		/// </summary>
		/// <param name="icon"></param>
		/// <param name="state"></param>
		void SetIcon(IIcon icon, eIconState state);

		IIcon GetIcon(eMainNavIcon iconType);

		IIcon GetIcon(eSourceType sourceType);
	}
}
