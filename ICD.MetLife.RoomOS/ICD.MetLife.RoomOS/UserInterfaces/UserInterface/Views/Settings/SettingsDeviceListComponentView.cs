using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsDeviceListComponentView : AbstractComponentView, ISettingsDeviceListComponentView
	{
		public event EventHandler OnPressed;
		public event EventHandler OnDeleteButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public SettingsDeviceListComponentView(ISigInputOutput panel, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;
			OnDeleteButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the label text for the component.
		/// </summary>
		/// <param name="label"></param>
		public void SetLabel(string label)
		{
			m_ItemButton.SetLabelTextAtJoin(m_ItemButton.SerialLabelJoins.First(), label);
		}

		/// <summary>
		/// Sets the visibility of the delete button.
		/// </summary>
		/// <param name="show"></param>
		public void ShowDeleteButton(bool show)
		{
			m_DeleteButton.Show(show);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DeleteButton.OnPressed += DeleteButtonOnPressed;
			m_ItemButton.OnPressed += ItemButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DeleteButton.OnPressed -= DeleteButtonOnPressed;
			m_ItemButton.OnPressed -= ItemButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the item button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ItemButtonOnPressed(object sender, EventArgs args)
		{
			OnPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the delete button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DeleteButtonOnPressed(object sender, EventArgs args)
		{
			OnDeleteButtonPressed.Raise(this);
		}

		#endregion
	}
}
