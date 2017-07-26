using System;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner.NavSource;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TvTuner.NavSource
{
	public abstract class AbstractNavSourceView : AbstractView, INavSourceView
	{
		public event EventHandler OnStopButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		protected AbstractNavSourceView(ISigInputOutput panel)
			: base(panel)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnStopButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the enabled state of the stop button.
		/// </summary>
		/// <param name="enabled"></param>
		public abstract void EnableStopButton(bool enabled);

		/// <summary>
		/// Raised the OnStopButtonPressed event.
		/// </summary>
		protected void RaiseOnStopButtonPressed()
		{
			OnStopButtonPressed.Raise(this);
		}
	}
}
