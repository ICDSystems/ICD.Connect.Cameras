using System;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Layout;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Layout
{
	public sealed partial class LayoutView : AbstractView, ILayoutView
	{
		public event EventHandler OnAutoButtonPressed;
		public event EventHandler OnEqualButtonPressed;
		public event EventHandler OnProminentButtonPressed;
		public event EventHandler OnOverlayButtonPressed;
		public event EventHandler OnSingleButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public LayoutView(ISigInputOutput panel)
			: base(panel)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnAutoButtonPressed = null;
			OnEqualButtonPressed = null;
			OnProminentButtonPressed = null;
			OnOverlayButtonPressed = null;
			OnSingleButtonPressed = null;

			base.Dispose();
		}

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_AutoButton.OnPressed += AutoButtonOnPressed;
			m_EqualButton.OnPressed += EqualButtonOnPressed;
			m_OverlayButton.OnPressed += OverlayButtonOnPressed;
			m_ProminentButton.OnPressed += ProminentButtonOnPressed;
			m_SingleButton.OnPressed += SingleButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_AutoButton.OnPressed -= AutoButtonOnPressed;
			m_EqualButton.OnPressed -= EqualButtonOnPressed;
			m_OverlayButton.OnPressed -= OverlayButtonOnPressed;
			m_ProminentButton.OnPressed -= ProminentButtonOnPressed;
			m_SingleButton.OnPressed -= SingleButtonOnPressed;
		}

		private void SingleButtonOnPressed(object sender, EventArgs args)
		{
			OnSingleButtonPressed.Raise(this);
		}

		private void ProminentButtonOnPressed(object sender, EventArgs args)
		{
			OnProminentButtonPressed.Raise(this);
		}

		private void OverlayButtonOnPressed(object sender, EventArgs args)
		{
			OnOverlayButtonPressed.Raise(this);
		}

		private void EqualButtonOnPressed(object sender, EventArgs args)
		{
			OnEqualButtonPressed.Raise(this);
		}

		private void AutoButtonOnPressed(object sender, EventArgs args)
		{
			OnAutoButtonPressed.Raise(this);
		}

		#endregion
	}
}
