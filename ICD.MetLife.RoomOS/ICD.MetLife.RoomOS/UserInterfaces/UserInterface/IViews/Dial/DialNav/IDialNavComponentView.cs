using System;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial.DialNav
{
	public interface IDialNavComponentView : IView
	{
		/// <summary>
		/// Called when the user presses the component.
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

		/// <summary>
		/// Gets the icon for the given type.
		/// </summary>
		/// <param name="iconType"></param>
		/// <returns></returns>
		IIcon GetIcon(eDialNavIcon iconType);
	}
}
