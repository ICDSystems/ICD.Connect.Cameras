using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial
{
	public sealed partial class KeyboardSpecialView : AbstractView, IKeyboardSpecialView
	{
		public event EventHandler OnAlphabetButtonPressed;
		public event KeyboardKeyPressedCallback OnKeyPressed;

		private Dictionary<VtProButton, KeyboardKey> m_KeyMap;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public KeyboardSpecialView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnAlphabetButtonPressed = null;
			OnKeyPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the shift state of the special chars.
		/// </summary>
		/// <param name="shift"></param>
		public void SetShift(bool shift)
		{
			// All keys share the same join for shift
			m_Key0Button.SetLabelTextAtJoin(m_Key0Button.DigitalLabelJoins.First(), shift);
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

			m_KeyMap[m_Key0Button] = new KeyboardKey('0', ')');
			m_KeyMap[m_Key1Button] = new KeyboardKey('1', '!');
			m_KeyMap[m_Key2Button] = new KeyboardKey('2', '@');
			m_KeyMap[m_Key3Button] = new KeyboardKey('3', '#');
			m_KeyMap[m_Key4Button] = new KeyboardKey('4', '$');
			m_KeyMap[m_Key5Button] = new KeyboardKey('5', '%');
			m_KeyMap[m_Key6Button] = new KeyboardKey('6', '^');
			m_KeyMap[m_Key7Button] = new KeyboardKey('7', '&');
			m_KeyMap[m_Key8Button] = new KeyboardKey('8', '*');
			m_KeyMap[m_Key9Button] = new KeyboardKey('9', '(');

			m_KeyMap[m_KeyDashButton] = new KeyboardKey('-', '_');
			m_KeyMap[m_KeyEqualsButton] = new KeyboardKey('=', '+');
			m_KeyMap[m_KeyBackslashButton] = new KeyboardKey('\\', '|');
			m_KeyMap[m_KeyColonButton] = new KeyboardKey(';', ':');
			m_KeyMap[m_KeyApostrapheButton] = new KeyboardKey('\'', '"');
			m_KeyMap[m_KeyCommaButton] = new KeyboardKey(',', '<');
			m_KeyMap[m_KeyPeriodButton] = new KeyboardKey('.', '>');
			m_KeyMap[m_KeyOpenBracketButton] = new KeyboardKey('[', '{');
			m_KeyMap[m_KeyCloseBracketButton] = new KeyboardKey(']', '}');

			foreach (VtProButton button in m_KeyMap.Keys)
				button.OnPressed += ButtonOnPressed;

			m_AlphabetButton.OnPressed += AlphabetButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			foreach (VtProButton button in m_KeyMap.Keys)
				button.OnPressed -= ButtonOnPressed;

			m_AlphabetButton.OnPressed -= AlphabetButtonOnPressed;
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
		/// Called when the alphabet button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void AlphabetButtonOnPressed(object sender, EventArgs args)
		{
			OnAlphabetButtonPressed.Raise(this);
		}

		#endregion
	}
}
