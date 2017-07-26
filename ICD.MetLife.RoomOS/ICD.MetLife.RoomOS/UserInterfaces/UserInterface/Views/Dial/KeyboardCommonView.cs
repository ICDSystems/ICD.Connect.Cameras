using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial
{
	public sealed partial class KeyboardCommonView : AbstractView, IKeyboardCommonView
	{
		public event EventHandler<StringEventArgs> OnTextEntered;
		public event EventHandler OnBackspaceButtonPressed;
		public event EventHandler OnClearButtonPressed;
		public event EventHandler OnSpaceButtonPressed;
		public event EventHandler OnCapsButtonPressed;
		public event EventHandler OnShiftButtonPressed;
		public event EventHandler OnDialButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public KeyboardCommonView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnTextEntered = null;
			OnBackspaceButtonPressed = null;
			OnClearButtonPressed = null;
			OnSpaceButtonPressed = null;
			OnCapsButtonPressed = null;
			OnShiftButtonPressed = null;
			OnDialButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the selected state of the caps button.
		/// </summary>
		public void SelectCapsButton(bool select)
		{
			m_CapsButton.SetSelected(select);
		}

		/// <summary>
		/// Sets the selected state of the shift button.
		/// </summary>
		public void SelectShiftButton(bool select)
		{
			m_ShiftButton.SetSelected(select);
		}

		/// <summary>
		/// Sets the text in the text entry field.
		/// </summary>
		/// <param name="text"></param>
		public void SetText(string text)
		{
			m_TextEntry.SetLabelTextAtJoin(m_TextEntry.SerialLabelJoins.First(), text);
		}

		/// <summary>
		/// Sets the call type label on the dial button.
		/// </summary>
		/// <param name="text"></param>
		public void SetDialTypeText(string text)
		{
			m_DialButton.SetLabelTextAtJoin(m_DialButton.SerialLabelJoins.First(), text);
		}

		/// <summary>
		/// Sets the enabled state of the dial button.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableDialButton(bool enable)
		{
			m_DialButton.Enable(enable);
		}

		/// <summary>
		/// Sets the enabled state of the backspace button.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableBackspaceButton(bool enable)
		{
			m_BackspaceButton.Enable(enable);
		}

		/// <summary>
		/// Sets the enabled state of the clear button.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableClearButton(bool enable)
		{
			m_ClearButton.Enable(enable);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_TextEntry.OnTextModified += TextEntryOnTextModified;
			m_BackspaceButton.OnPressed += BackspaceButtonOnPressed;
			m_ClearButton.OnPressed += ClearButtonOnPressed;
			m_SpaceButton.OnPressed += SpaceButtonOnPressed;
			m_CapsButton.OnPressed += CapsButtonOnPressed;
			m_ShiftButton.OnPressed += ShiftButtonOnPressed;
			m_DialButton.OnPressed += DialButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_TextEntry.OnTextModified -= TextEntryOnTextModified;
			m_BackspaceButton.OnPressed -= BackspaceButtonOnPressed;
			m_ClearButton.OnPressed -= ClearButtonOnPressed;
			m_SpaceButton.OnPressed -= SpaceButtonOnPressed;
			m_CapsButton.OnPressed -= CapsButtonOnPressed;
			m_ShiftButton.OnPressed -= ShiftButtonOnPressed;
			m_DialButton.OnPressed -= DialButtonOnPressed;
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

		/// <summary>
		/// Called when the user presses the shift button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ShiftButtonOnPressed(object sender, EventArgs args)
		{
			OnShiftButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the caps button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CapsButtonOnPressed(object sender, EventArgs args)
		{
			OnCapsButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the space button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SpaceButtonOnPressed(object sender, EventArgs args)
		{
			OnSpaceButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the clear button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ClearButtonOnPressed(object sender, EventArgs args)
		{
			OnClearButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the backspace button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void BackspaceButtonOnPressed(object sender, EventArgs args)
		{
			OnBackspaceButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user enters text in the text field.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void TextEntryOnTextModified(object sender, StringEventArgs args)
		{
			OnTextEntered.Raise(this, new StringEventArgs(args.Data));
		}

		#endregion
	}
}
