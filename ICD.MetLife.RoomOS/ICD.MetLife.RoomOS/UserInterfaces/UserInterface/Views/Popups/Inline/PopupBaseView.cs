using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline
{
	public sealed partial class PopupBaseView : AbstractView, IPopupBaseView
	{
		public event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public PopupBaseView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCloseButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the title text.
		/// </summary>
		/// <param name="label"></param>
		public void SetTitle(string label)
		{
			m_TitleLabel.SetLabelTextAtJoin(m_TitleLabel.SerialLabelJoins.First(), label);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
		}

		/// <summary>
		/// Called when the close button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CloseButtonOnPressed(object sender, EventArgs args)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}
