using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Connect.UI.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TouchTones;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TouchTones
{
	public sealed partial class TouchTonesView : AbstractView, ITouchTonesView
	{
		public event EventHandler<UShortEventArgs> OnContactButtonPressed;
		public event EventHandler<CharEventArgs> OnToneButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public TouchTonesView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnContactButtonPressed = null;
			OnToneButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the contact name labels.
		/// </summary>
		/// <param name="names"></param>
		public void SetContactNames(IEnumerable<string> names)
		{
			string[] namesArray = names.Take(m_ContactList.MaxSize).ToArray();
			m_ContactList.SetItemLabels(namesArray);
		}

		/// <summary>
		/// Sets the selected state of the contact at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetContactSelected(ushort index, bool selected)
		{
			if (index < m_ContactList.MaxSize)
				m_ContactList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the visibility of the contact buttons.
		/// </summary>
		/// <param name="visible"></param>
		public void SetContactsButtonsVisible(bool visible)
		{
			m_ContactList.Show(visible);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ContactList.OnButtonClicked += ContactListOnButtonClicked;
			m_Keypad.OnButtonPressed += KeypadOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ContactList.OnButtonClicked -= ContactListOnButtonClicked;
			m_Keypad.OnButtonPressed -= KeypadOnButtonPressed;
		}

		/// <summary>
		/// Called when a contact button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ContactListOnButtonClicked(object sender, UShortEventArgs args)
		{
			OnContactButtonPressed.Raise(this, new UShortEventArgs(args.Data));
		}

		/// <summary>
		/// Called when a keypad button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void KeypadOnButtonPressed(object sender, SimpleKeypadEventArgs args)
		{
			char character = m_Keypad.GetButtonChar(args.Data);
			OnToneButtonPressed.Raise(this, new CharEventArgs(character));
		}

		#endregion
	}
}
