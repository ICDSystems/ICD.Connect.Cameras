using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking.Keyboard;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Blocking.Keyboard
{
	public sealed partial class PopupKeyboardAlphaView : AbstractView, IPopupKeyboardAlphaView
	{
		public event EventHandler OnSpecialButtonPressed;
		public event PopupKeyboardKeyPressedCallback OnKeyPressed;

		private Dictionary<VtProButton, KeyboardKey> m_KeyMap;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public PopupKeyboardAlphaView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnSpecialButtonPressed = null;
			OnKeyPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the shift state of the chars.
		/// </summary>
		/// <param name="shift"></param>
		/// <param name="caps"></param>
		public void SetShift(bool shift, bool caps)
		{
			// All letters use the same join
			m_KeyQButton.SetLabelTextAtJoin(m_KeyQButton.DigitalLabelJoins.First(), shift ^ caps);

			// Except the slash key...
			m_KeySlashButton.SetLabelTextAtJoin(m_KeySlashButton.DigitalLabelJoins.First(), shift);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_KeyMap = new Dictionary<VtProButton, KeyboardKey>();

			m_KeyMap[m_KeyQButton] = new KeyboardKey('q');
			m_KeyMap[m_KeyWButton] = new KeyboardKey('w');
			m_KeyMap[m_KeyEButton] = new KeyboardKey('e');
			m_KeyMap[m_KeyRButton] = new KeyboardKey('r');
			m_KeyMap[m_KeyTButton] = new KeyboardKey('t');
			m_KeyMap[m_KeyYButton] = new KeyboardKey('y');
			m_KeyMap[m_KeyUButton] = new KeyboardKey('u');
			m_KeyMap[m_KeyIButton] = new KeyboardKey('i');
			m_KeyMap[m_KeyOButton] = new KeyboardKey('o');
			m_KeyMap[m_KeyPButton] = new KeyboardKey('p');

			m_KeyMap[m_KeyAButton] = new KeyboardKey('a');
			m_KeyMap[m_KeySButton] = new KeyboardKey('s');
			m_KeyMap[m_KeyDButton] = new KeyboardKey('d');
			m_KeyMap[m_KeyFButton] = new KeyboardKey('f');
			m_KeyMap[m_KeyGButton] = new KeyboardKey('g');
			m_KeyMap[m_KeyHButton] = new KeyboardKey('h');
			m_KeyMap[m_KeyJButton] = new KeyboardKey('j');
			m_KeyMap[m_KeyKButton] = new KeyboardKey('k');
			m_KeyMap[m_KeyLButton] = new KeyboardKey('l');

			m_KeyMap[m_KeyZButton] = new KeyboardKey('z');
			m_KeyMap[m_KeyXButton] = new KeyboardKey('x');
			m_KeyMap[m_KeyCButton] = new KeyboardKey('c');
			m_KeyMap[m_KeyVButton] = new KeyboardKey('v');
			m_KeyMap[m_KeyBButton] = new KeyboardKey('b');
			m_KeyMap[m_KeyNButton] = new KeyboardKey('n');
			m_KeyMap[m_KeyMButton] = new KeyboardKey('m');

			m_KeyMap[m_KeySlashButton] = new KeyboardKey('/', '?');
			m_KeyMap[m_KeyAtButton] = new KeyboardKey('@', '@');
			m_KeyMap[m_KeyStopButton] = new KeyboardKey('.', '.');

			foreach (VtProButton button in m_KeyMap.Keys)
				button.OnPressed += ButtonOnPressed;

			m_SpecialButton.OnPressed += SpecialButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			foreach (VtProButton button in m_KeyMap.Keys)
				button.OnPressed -= ButtonOnPressed;

			m_SpecialButton.OnPressed -= SpecialButtonOnPressed;
		}

		/// <summary>
		/// Called when a key button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ButtonOnPressed(object sender, EventArgs args)
		{
			if (OnKeyPressed != null)
				OnKeyPressed(this, m_KeyMap[sender as VtProButton]);
		}

		/// <summary>
		/// Called when the user presses the special button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SpecialButtonOnPressed(object sender, EventArgs args)
		{
			OnSpecialButtonPressed.Raise(this);
		}

		#endregion
	}
}
