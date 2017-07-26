using System;
using ICD.MetLife.RoomOS.Endpoints.Sources;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.ShareMenu
{
	/// <summary>
	/// Represents a source in the share menu.
	/// </summary>
	public interface IShareComponentView : IView
	{
		/// <summary>
		/// Raised when the user presses the component.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Raised when the user releases the component.
		/// </summary>
		event EventHandler OnReleased;

		/// <summary>
		/// Sets the source label.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="status"></param>
		void SetLabel(string name, string status);

		/// <summary>
		/// Sets the icon for the source.
		/// </summary>
		/// <param name="icon"></param>
		/// <param name="state"></param>
		void SetIcon(IIcon icon, eIconState state);

		IIcon GetIcon(eSourceType sourceType);
	}
}
