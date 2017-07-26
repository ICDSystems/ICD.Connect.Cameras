using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking.Keyboard;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Blocking.Keyboard
{
	public sealed partial class PopupKeyboardCommonView : AbstractView, IPopupKeyboardCommonView
	{
		public event EventHandler<StringEventArgs> OnTextEntered;
		public event EventHandler OnBackspaceButtonPressed;
		public event EventHandler OnClearButtonPressed;
		public event EventHandler OnSpaceButtonPressed;
		public event EventHandler OnCapsButtonPressed;
		public event EventHandler OnShiftButtonPressed;
		public event EventHandler OnSubmitButtonPressed;
		public event EventHandler OnCancelButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public PopupKeyboardCommonView(ISigInputOutput panel)
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
			OnSubmitButtonPressed = null;
			OnCancelButtonPressed = null;

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
		/// Sets the label text on the submit button.
		/// </summary>
		/// <param name="text"></param>
		public void SetSubmitButtonLabel(string text)
		{
			m_SubmitButton.SetLabelTextAtJoin(m_SubmitButton.SerialLabelJoins.First(), text);
		}

		/// <summary>
		/// Sets the label text on the cancel button.
		/// </summary>
		/// <param name="text"></param>
		public void SetCancelButtonLabel(string text)
		{
			m_CancelButton.SetLabelTextAtJoin(m_CancelButton.SerialLabelJoins.First(), text);
		}

		/// <summary>
		/// Sets the enabled state of the dial button.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableSubmitButton(bool enable)
		{
			m_SubmitButton.Enable(enable);
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
			m_SubmitButton.OnPressed += SubmitButtonOnPressed;
			m_CancelButton.OnPressed += CancelButtonOnPressed;
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
			m_SubmitButton.OnPressed -= SubmitButtonOnPressed;
			m_CancelButton.OnPressed -= CancelButtonOnPressed;
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

		/// <summary>
		/// Called when the user presses the cancel button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CancelButtonOnPressed(object sender, EventArgs args)
		{
			OnCancelButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the submit button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SubmitButtonOnPressed(object sender, EventArgs args)
		{
			OnSubmitButtonPressed.Raise(this);
		}

		#endregion
	}
}
