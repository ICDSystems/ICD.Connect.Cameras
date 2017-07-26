using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TouchTones
{
	public interface ITouchTonesView : IView
	{
		/// <summary>
		/// Raised when the user presses a contact button.
		/// </summary>
		event EventHandler<UShortEventArgs> OnContactButtonPressed;

		/// <summary>
		/// Raised when the user presses a dialtone button.
		/// </summary>
		event EventHandler<CharEventArgs> OnToneButtonPressed;

		/// <summary>
		/// Sets the contact name labels.
		/// </summary>
		/// <param name="names"></param>
		void SetContactNames(IEnumerable<string> names);

		/// <summary>
		/// Sets the selected state of the contact at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetContactSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the visibility of the contact buttons.
		/// </summary>
		/// <param name="visible"></param>
		void SetContactsButtonsVisible(bool visible);
	}
}
