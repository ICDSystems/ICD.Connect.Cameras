using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Lights
{
	public sealed partial class ShadeComponentView : AbstractComponentView, IShadeComponentView
	{
		public event EventHandler OnUpButtonPressed;
		public event EventHandler OnDownButtonPressed;
		public event EventHandler OnStopButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ShadeComponentView(ISigInputOutput panel, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnUpButtonPressed = null;
			OnDownButtonPressed = null;
			OnStopButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the title text.
		/// </summary>
		/// <param name="title"></param>
		public void SetTitle(string title)
		{
			m_Title.SetLabelTextAtJoin(m_Title.SerialLabelJoins.First(), title);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DownButton.OnPressed += DownButtonOnPressed;
			m_UpButton.OnPressed += UpButtonOnPressed;
			m_StopButton.OnPressed += StopButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DownButton.OnPressed -= DownButtonOnPressed;
			m_UpButton.OnPressed -= UpButtonOnPressed;
			m_StopButton.OnPressed -= StopButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void DownButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDownButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the stop button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StopButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnStopButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void UpButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnUpButtonPressed.Raise(this);
		}

		#endregion
	}
}
