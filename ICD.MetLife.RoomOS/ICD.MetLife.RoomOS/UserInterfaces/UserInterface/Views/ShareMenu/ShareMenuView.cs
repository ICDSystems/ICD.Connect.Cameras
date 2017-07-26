using System;
using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.ShareMenu;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.ShareMenu
{
	public sealed partial class ShareMenuView : AbstractView, IShareMenuView
	{
		public event EventHandler OnStopSharingButtonPressed;

		private readonly List<IShareComponentView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public ShareMenuView(ISigInputOutput panel)
			: base(panel)
		{
			m_ChildList = new List<IShareComponentView>();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnStopSharingButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IShareComponentView> GetChildCallViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_SourceList, m_ChildList, count);
		}

		/// <summary>
		/// Sets the enabled state of the stop sharing button.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableStopSharingButton(bool enable)
		{
			m_StopSharingButton.Enable(enable);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_StopSharingButton.OnPressed += StopSharingButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_StopSharingButton.OnPressed -= StopSharingButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the stop sharing button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void StopSharingButtonOnPressed(object sender, EventArgs args)
		{
			OnStopSharingButtonPressed.Raise(this);
		}

		#endregion
	}
}
