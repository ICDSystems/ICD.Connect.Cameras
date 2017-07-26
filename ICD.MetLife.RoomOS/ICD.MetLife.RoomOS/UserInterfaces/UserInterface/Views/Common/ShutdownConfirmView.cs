using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Common;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Common
{
	public sealed partial class ShutdownConfirmView : AbstractView, IShutdownConfirmView
	{
		public event EventHandler OnCancelButtonPressed;
		public event EventHandler OnShutdownButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public ShutdownConfirmView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCancelButtonPressed = null;
			OnShutdownButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the number of remaining seconds until shutdown.
		/// </summary>
		/// <param name="seconds"></param>
		public void SetRemainingSeconds(ushort seconds)
		{
			m_ShutdownMessage.SetLabelTextAtJoin(m_ShutdownMessage.AnalogLabelJoins.First(), seconds);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CancelButton.OnPressed += CancelButtonOnPressed;
			m_ShutdownButton.OnPressed += ShutdownButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CancelButton.OnPressed -= CancelButtonOnPressed;
			m_ShutdownButton.OnPressed -= ShutdownButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the shutdown button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ShutdownButtonOnPressed(object sender, EventArgs args)
		{
			OnShutdownButtonPressed.Raise(this);
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

		#endregion
	}
}
