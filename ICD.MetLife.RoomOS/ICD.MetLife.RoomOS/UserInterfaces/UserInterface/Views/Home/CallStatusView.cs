using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Home;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Home
{
	public sealed partial class CallStatusView : AbstractComponentView, ICallStatusView
	{
		public event EventHandler OnEndCallButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public CallStatusView(ISigInputOutput panel, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnEndCallButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the call type/status text.
		/// </summary>
		/// <param name="status"></param>
		/// <param name="color"></param>
		public void SetCallStatusText(string status, eColor color)
		{
			string colorHex = VtProColors.ColorToHexString(color);
			status = HtmlUtils.FormatColoredText(status, colorHex);

			m_StatusText.SetLabelTextAtJoin(m_StatusText.SerialLabelJoins.First(), status);
		}

		/// <summary>
		/// Sets the call number/name text.
		/// </summary>
		/// <param name="name"></param>
		public void SetCallName(string name)
		{
			m_StatusText.SetLabelTextAtJoin(m_StatusText.SerialLabelJoins.Skip(1).First(), name);
		}

		/// <summary>
		/// Sets the call duration in seconds.
		/// </summary>
		/// <param name="seconds"></param>
		public void SetCallDuration(ushort seconds)
		{
			m_StatusText.SetLabelTextAtJoin(m_StatusText.AnalogLabelJoins.First(), seconds);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_EndCallButton.OnPressed += EndCallButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_EndCallButton.OnPressed -= EndCallButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the end call button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void EndCallButtonOnPressed(object sender, EventArgs args)
		{
			OnEndCallButtonPressed.Raise(this);
		}

		#endregion
	}
}
