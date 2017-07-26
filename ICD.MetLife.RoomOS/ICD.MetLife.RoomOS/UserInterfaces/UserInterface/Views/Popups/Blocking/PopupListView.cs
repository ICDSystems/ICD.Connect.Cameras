using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Blocking
{
	public sealed partial class PopupListView : AbstractView, IPopupListView
	{
		public event EventHandler OnCloseButtonPressed;
		public event EventHandler<UShortEventArgs> OnItemButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public PopupListView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Sets the label for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		public void SetItemLabel(ushort index, string label)
		{
			m_ItemList.SetItemLabel(index, label);
		}

		/// <summary>
		/// Sets the selection state for the item at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public void SetItemSelected(ushort index, bool selected)
		{
			m_ItemList.SetItemSelected(index, selected);
		}

		/// <summary>
		/// Sets the title text for the popup.
		/// </summary>
		/// <param name="title"></param>
		public void SetTitle(string title)
		{
			m_TitleLabel.SetLabelTextAtJoin(m_TitleLabel.SerialLabelJoins.First(), title);
		}

		/// <summary>
		/// Sets the number of items in the button list.
		/// </summary>
		/// <param name="count"></param>
		public void SetItemCount(ushort count)
		{
			m_ItemList.SetNumberOfItems(count);
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
			m_ItemList.OnButtonClicked += ItemListOnButtonClicked;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ExitButton.OnPressed -= ExitButtonOnPressed;
			m_ItemList.OnButtonClicked -= ItemListOnButtonClicked;
		}

		/// <summary>
		/// Called when the user presses an item in the list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ItemListOnButtonClicked(object sender, UShortEventArgs args)
		{
			OnItemButtonPressed.Raise(this, new UShortEventArgs(args.Data));
		}

		/// <summary>
		/// Called when the user presses the exit button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ExitButtonOnPressed(object sender, EventArgs args)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}
