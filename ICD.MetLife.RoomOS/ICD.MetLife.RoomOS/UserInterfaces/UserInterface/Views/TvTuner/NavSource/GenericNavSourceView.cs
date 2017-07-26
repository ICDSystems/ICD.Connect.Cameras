using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner.NavSource;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TvTuner.NavSource
{
	public sealed partial class GenericNavSourceView : AbstractNavSourceView, IGenericNavSourceView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public GenericNavSourceView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		public void SetSourcePresentParticiple(string verb)
		{
			m_StopButton.SetLabelTextAtJoin(m_StopButton.SerialLabelJoins.First(), verb);
		}

		/// <summary>
		/// Sets the name of the source currently routed to the display.
		/// </summary>
		/// <param name="name"></param>
		public void SetSourceName(string name)
		{
			m_StopButton.SetLabelTextAtJoin(m_StopButton.SerialLabelJoins.ElementAt(1), name);
		}

		/// <summary>
		/// Sets the enabled state of the stop button.
		/// </summary>
		/// <param name="enabled"></param>
		public override void EnableStopButton(bool enabled)
		{
			m_StopButton.Enable(enabled);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_StopButton.OnPressed += StopButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_StopButton.OnPressed -= StopButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the stop button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StopButtonOnPressed(object sender, EventArgs eventArgs)
		{
			RaiseOnStopButtonPressed();
		}

		#endregion
	}
}
