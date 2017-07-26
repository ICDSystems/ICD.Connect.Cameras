using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Blocking;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Popups.Blocking
{
	public sealed class PopupListPresenter : AbstractPresenter<IPopupListView>, IPopupListPresenter
	{
		private readonly List<object> m_Items;
		private readonly Dictionary<object, bool> m_ItemSelection; 

		private Func<object, string> m_GetLabelCallback;
		private string m_Title;
		private Action<object> m_ItemPressedCallback;
		private Func<object, bool> m_GetSelectedCallback;
		private Action m_ClosedCallback;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public PopupListPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_Items = new List<object>();
			m_ItemSelection = new Dictionary<object, bool>();
		}

		#region Methods

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IPopupListView view)
		{
			base.Refresh(view);

			view.SetTitle(m_Title);
			view.SetItemCount((ushort)m_Items.Count);

			for (ushort index = 0; index < m_Items.Count; index++)
			{
				object item = m_Items[index];
				view.SetItemLabel(index, m_GetLabelCallback(item));
				view.SetItemSelected(index, GetItemSelected(item));
			}
		}

		/// <summary>
		/// Clears the list.
		/// </summary>
		public void Clear()
		{
			this.SetListItems(Enumerable.Empty<object>(), null, null, null);
		}

		/// <summary>
		/// Populates the list.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="getLabel"></param>
		/// <param name="getSelected"></param>
		/// <param name="title"></param>
		/// <param name="itemPressedCallback"></param>
		/// <param name="closedCallback"></param>
		public void SetListItems(IEnumerable<object> items, Func<object, string> getLabel, Func<object, bool> getSelected,
		                         string title, Action<object> itemPressedCallback, Action closedCallback)
		{
			m_GetLabelCallback = getLabel;
			m_GetSelectedCallback = getSelected;
			m_Title = title;
			m_ItemPressedCallback = itemPressedCallback;
			m_ClosedCallback = closedCallback;

			m_Items.Clear();
			m_Items.AddRange(items);

			m_ItemSelection.Clear();
			m_ItemSelection.AddRange(m_Items.Where(i => i != null), m_GetSelectedCallback);

			RefreshIfVisible();
		}

		/// <summary>
		/// Sets the selection state of the given item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="selected"></param>
		public void SetItemSelected(object item, bool selected)
		{
			if (item == null)
				return;

			if (selected == GetItemSelected(item))
				return;

			m_ItemSelection[item] = selected;

			RefreshIfVisible();
		}

		/// <summary>
		/// Returns true if the given item is selected.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool GetItemSelected(object item)
		{
			return item != null && m_ItemSelection.GetDefault(item, false);
		}

		/// <summary>
		/// Returns the items that are selected.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<object> GetSelectedItems()
		{
			return m_ItemSelection.Where(kvp => kvp.Value)
			                      .Select(kvp => kvp.Key)
			                      .ToArray();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IPopupListView view)
		{
			base.Subscribe(view);

			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
			view.OnItemButtonPressed += ViewOnItemButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IPopupListView view)
		{
			base.Unsubscribe(view);

			view.OnCloseButtonPressed -= ViewOnCloseButtonPressed;
			view.OnItemButtonPressed -= ViewOnItemButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		/// <summary>
		/// Called when the user
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ViewOnItemButtonPressed(object sender, UShortEventArgs args)
		{
			ushort index = args.Data;

			if (m_ItemPressedCallback != null)
				m_ItemPressedCallback(m_Items[index]);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (args.Data)
				return;

			// Call the closed callback before clearing so the parent can get selected values
			m_ClosedCallback();
			Clear();
		}

		#endregion
	}
}
