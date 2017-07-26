using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Connect.UI.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial
{
	public sealed partial class DialpadView : AbstractView, IDialpadView
	{
		public event EventHandler OnDialButtonPressed;
		public event EventHandler OnBackspaceButtonPressed;
		public event EventHandler OnClearButtonPressed;
		public event EventHandler<CharEventArgs> OnKeypadButtonPressed;
		public event EventHandler<StringEventArgs> OnTextEntryModified;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public DialpadView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDialButtonPressed = null;
			OnBackspaceButtonPressed = null;
			OnClearButtonPressed = null;
			OnKeypadButtonPressed = null;
			OnTextEntryModified = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the call type text in the dial button (i.e. Audio, Video)
		/// </summary>
		/// <param name="type"></param>
		public void SetCallTypeLabel(string type)
		{
			m_DialButton.SetLabelTextAtJoin(m_DialButton.SerialLabelJoins.First(), type);
		}

		/// <summary>
		/// Sets the text in the text entry field.
		/// </summary>
		/// <param name="text"></param>
		public void SetTextEntryText(string text)
		{
			m_TextEntry.SetLabelTextAtJoin(m_TextEntry.SerialLabelJoins.First(), text);
		}

		/// <summary>
		/// Sets the visibility of the clear button.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableClearButton(bool enable)
		{
			m_ClearButton.Enable(enable);
		}

		/// <summary>
		/// Sets the visibility of the backspace button.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableBackspaceButton(bool enable)
		{
			m_BackspaceButton.Enable(enable);
		}

		/// <summary>
		/// Enables/disables the dial button.
		/// </summary>
		/// <param name="enabled"></param>
		public void EnableDialButton(bool enabled)
		{
			m_DialButton.Enable(enabled);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DialButton.OnPressed += DialButtonOnPressed;
			m_BackspaceButton.OnPressed += BackspaceButtonOnPressed;
			m_ClearButton.OnPressed += ClearButtonOnPressed;
			m_Keypad.OnButtonPressed += KeypadOnButtonPressed;
			m_TextEntry.OnTextModified += TextEntryOnTextModified;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DialButton.OnPressed -= DialButtonOnPressed;
			m_BackspaceButton.OnPressed -= BackspaceButtonOnPressed;
			m_ClearButton.OnPressed -= ClearButtonOnPressed;
			m_Keypad.OnButtonPressed -= KeypadOnButtonPressed;
			m_TextEntry.OnTextModified -= TextEntryOnTextModified;
		}

		/// <summary>
		/// Called when the user presses the dial button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DialButtonOnPressed(object sender, EventArgs args)
		{
			OnDialButtonPressed.Raise(this);
		}

		private void ClearButtonOnPressed(object sender, EventArgs args)
		{
			OnClearButtonPressed.Raise(this);
		}

		private void BackspaceButtonOnPressed(object sender, EventArgs args)
		{
			OnBackspaceButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses a keypad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="simpleKeypadEventArgs"></param>
		private void KeypadOnButtonPressed(object sender, SimpleKeypadEventArgs simpleKeypadEventArgs)
		{
			char character = m_Keypad.GetButtonChar(simpleKeypadEventArgs.Data);
			OnKeypadButtonPressed.Raise(this, new CharEventArgs(character));
		}

		/// <summary>
		/// Called when the user enters text in the text field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void TextEntryOnTextModified(object sender, StringEventArgs args)
		{
			OnTextEntryModified.Raise(this, new StringEventArgs(args.Data));
		}

		#endregion
	}
}
