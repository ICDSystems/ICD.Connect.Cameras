using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial
{
	public sealed partial class RecentsView : AbstractView, IRecentsView
	{
		public event EventHandler OnRemoveFromListButtonPressed;
		public event EventHandler OnDialButtonPressed;

		private readonly List<IRecentCallView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public RecentsView(ISigInputOutput panel)
			: base(panel)
		{
			m_ChildList = new List<IRecentCallView>();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnRemoveFromListButtonPressed = null;
			OnDialButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the call type text on the dial button (i.e. Audio, Video)
		/// </summary>
		/// <param name="type"></param>
		public void SetCallTypeText(string type)
		{
			m_DialButton.SetLabelTextAtJoin(m_DialButton.SerialLabelJoins.First(), type);
		}

		/// <summary>
		/// Sets the visibility of the remove from list button.
		/// </summary>
		/// <param name="show"></param>
		public void ShowRemoveFromListButton(bool show)
		{
			m_RemoveButton.Show(show);
		}

		/// <summary>
		/// Sets the enabled state of the dial button.
		/// </summary>
		/// <param name="enabled"></param>
		public void EnabledDialButton(bool enabled)
		{
			m_DialButton.Enable(enabled);
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IRecentCallView> GetChildCallViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_RecentCallsList, m_ChildList, count);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DialButton.OnPressed += DialButtonOnPressed;
			m_RemoveButton.OnPressed += RemoveButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DialButton.OnPressed -= DialButtonOnPressed;
			m_RemoveButton.OnPressed -= RemoveButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the dial button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DialButtonOnPressed(object sender, EventArgs args)
		{
			OnDialButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the remove button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void RemoveButtonOnPressed(object sender, EventArgs args)
		{
			OnRemoveFromListButtonPressed.Raise(this);
		}

		#endregion
	}
}
