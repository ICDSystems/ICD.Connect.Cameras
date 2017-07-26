using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsDeviceListView : AbstractView, ISettingsDeviceListView
	{
		public event EventHandler OnExitButtonPressed;

		private readonly List<ISettingsDeviceListComponentView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public SettingsDeviceListView(ISigInputOutput panel)
			: base(panel)
		{
			m_ChildList = new List<ISettingsDeviceListComponentView>();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnExitButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the title label text.
		/// </summary>
		/// <param name="title"></param>
		public void SetTitle(string title)
		{
			m_TitleLabel.SetLabelTextAtJoin(m_TitleLabel.SerialLabelJoins.First(), title);
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<ISettingsDeviceListComponentView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_List, m_ChildList, count);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ExitButton.OnPressed += ExitButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ExitButton.OnPressed -= ExitButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the exit button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ExitButtonOnPressed(object sender, EventArgs args)
		{
			OnExitButtonPressed.Raise(this);
		}

		#endregion
	}
}
