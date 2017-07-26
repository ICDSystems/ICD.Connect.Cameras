using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Blocking
{
	public sealed partial class IncomingCallView : AbstractView, IIncomingCallView
	{
		public event EventHandler OnRejectButtonPressed;
		public event EventHandler OnAnswerButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public IncomingCallView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnRejectButtonPressed = null;
			OnAnswerButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the incoming/outgoing call text.
		/// </summary>
		/// <param name="message"></param>
		public void SetMessageText(string message)
		{
			m_MessageLabel.SetLabelTextAtJoin(m_MessageLabel.SerialLabelJoins.First(), message);
		}

		/// <summary>
		/// Sets the visibility of the accept button.
		/// </summary>
		/// <param name="show"></param>
		public void ShowAcceptButton(bool show)
		{
			m_AcceptButton.Show(show);
		}

		/// <summary>
		/// Sets the visibility of the cancel button.
		/// </summary>
		/// <param name="show"></param>
		public void ShowCancelButton(bool show)
		{
			m_CancelButton.Show(show);
		}

		/// <summary>
		/// Sets the visibility of the reject button.
		/// </summary>
		/// <param name="show"></param>
		public void ShowRejectButton(bool show)
		{
			m_RejectButton.Show(show);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_AcceptButton.OnPressed += AcceptButtonOnPressed;
			m_CancelButton.OnPressed += CancelButtonOnPressed;
			m_RejectButton.OnPressed += RejectButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_AcceptButton.OnPressed -= AcceptButtonOnPressed;
			m_CancelButton.OnPressed -= CancelButtonOnPressed;
			m_RejectButton.OnPressed -= RejectButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the reject button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void RejectButtonOnPressed(object sender, EventArgs args)
		{
			OnRejectButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the cancel button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CancelButtonOnPressed(object sender, EventArgs args)
		{
			OnRejectButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the accept button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void AcceptButtonOnPressed(object sender, EventArgs args)
		{
			OnAnswerButtonPressed.Raise(this);
		}

		#endregion
	}
}
