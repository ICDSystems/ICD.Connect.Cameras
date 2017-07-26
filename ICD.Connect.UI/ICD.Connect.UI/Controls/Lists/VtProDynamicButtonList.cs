using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.UI.Controls.Lists
{
	public sealed class VtProDynamicButtonList : AbstractVtProButtonList
	{
		private const ushort SCROLL_TO_ITEM_JOIN = 3;
		private const ushort NUMBER_OF_ITEMS_JOIN = 4;
		private const ushort ITEM_CLICKED_JOIN = 1;
		private const ushort ITEM_HELD_JOIN = 2;
		private const ushort DIGITAL_IS_MOVING_JOIN = 2;
		private const ushort START_SELECTED_JOIN = 11;
		private const ushort START_ENABLED_JOIN = 2011;
		private const ushort START_VISIBLE_JOIN = 4011;
		private const ushort START_TEXT_JOIN = 11;
		private const ushort START_ICON_JOIN = 2011;

		private readonly Dictionary<ushort, bool> m_VisibilityCache;
		private readonly Dictionary<ushort, bool> m_EnabledCache;
		private readonly Dictionary<ushort, bool> m_SelectedCache;
		private readonly Dictionary<ushort, string> m_LabelCache;
		private readonly Dictionary<ushort, string> m_IconCache;

		private readonly SafeCriticalSection m_SetSelectedSection;
		private readonly SafeCriticalSection m_SetEnabledSection;
		private readonly SafeCriticalSection m_SetVisibleSection;
		private readonly SafeCriticalSection m_SetTextSection;
		private readonly SafeCriticalSection m_SetIconSection;

		/// <summary>
		/// Called when a button is released.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnButtonReleased;

		/// <summary>
		/// Called when a button is pressed and released without being held.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnButtonClicked;

		/// <summary>
		/// Called when a button has been pressed for an amount of time determined by the panel.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnButtonHeld;

		/// <summary>
		/// Gets the join number for settings the number of items in the list.
		/// </summary>
		protected override ushort AnalogNumberOfItemsJoin { get { return NUMBER_OF_ITEMS_JOIN; } }

		/// <summary>
		/// Gets the join number for scrolling to an item in the list.
		/// </summary>
		protected override ushort AnalogScrollToItemJoin { get { return SCROLL_TO_ITEM_JOIN; } }

		/// <summary>
		/// Gets the join number for getting the moving state of the list.
		/// </summary>
		protected override ushort DigitalIsMovingOutputJoin { get { return DIGITAL_IS_MOVING_JOIN; } }

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		[PublicAPI]
		public VtProDynamicButtonList(uint smartObjectId, IPanelDevice panel)
			: this(smartObjectId, panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProDynamicButtonList(uint smartObjectId, IPanelDevice panel, IVtProParent parent)
			: base(smartObjectId, panel, parent)
		{
			m_VisibilityCache = new Dictionary<ushort, bool>();
			m_EnabledCache = new Dictionary<ushort, bool>();
			m_SelectedCache = new Dictionary<ushort, bool>();
			m_LabelCache = new Dictionary<ushort, string>();
			m_IconCache = new Dictionary<ushort, string>();

			m_SetSelectedSection = new SafeCriticalSection();
			m_SetEnabledSection = new SafeCriticalSection();
			m_SetVisibleSection = new SafeCriticalSection();
			m_SetTextSection = new SafeCriticalSection();
			m_SetIconSection = new SafeCriticalSection();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnButtonReleased = null;
			OnButtonClicked = null;
			OnButtonHeld = null;

			base.Dispose();
		}

		/// <summary>
		/// Simulates a release on the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		[PublicAPI]
		public void Release(ushort index)
		{
			if (IsVisibleRecursive)
				OnButtonReleased.Raise(this, new UShortEventArgs(index));
		}

		/// <summary>
		/// Simulates a click on the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		[PublicAPI]
		public void Click(ushort index)
		{
			if (IsVisibleRecursive)
				OnButtonClicked.Raise(this, new UShortEventArgs(index));
		}

		/// <summary>
		/// Simulates the button at the given index being held.
		/// </summary>
		/// <param name="index"></param>
		[PublicAPI]
		public void Hold(ushort index)
		{
			if (IsVisibleRecursive)
				OnButtonHeld.Raise(this, new UShortEventArgs(index));
		}

		/// <summary>
		/// Sets the visible state for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetItemVisible(ushort index, bool visible)
		{
			m_SetVisibleSection.Enter();

			try
			{
				if (visible == m_VisibilityCache.GetDefault(index, false))
					return;

				ushort key = (ushort)(START_VISIBLE_JOIN + index);
				SmartObject.SendInputDigital(key, visible);

				m_VisibilityCache[index] = visible;
			}
			finally
			{
				m_SetVisibleSection.Leave();
			}
		}

		/// <summary>
		/// Sets the enabled state for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="enabled"></param>
		[PublicAPI]
		public void SetItemEnabled(ushort index, bool enabled)
		{
			m_SetEnabledSection.Enter();

			try
			{
				if (enabled == m_EnabledCache.GetDefault(index, false))
					return;

				ushort key = (ushort)(START_ENABLED_JOIN + index);
				SmartObject.SendInputDigital(key, enabled);

				m_EnabledCache[index] = enabled;
			}
			finally
			{
				m_SetEnabledSection.Leave();
			}
		}

		/// <summary>
		/// Sets the selected state for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public override void SetItemSelected(ushort index, bool selected)
		{
			m_SetSelectedSection.Enter();

			try
			{
				if (selected == m_SelectedCache.GetDefault(index, false))
					return;

				ushort key = (ushort)(START_SELECTED_JOIN + index);
				SmartObject.SendInputDigital(key, selected);

				m_SelectedCache[index] = selected;
			}
			finally
			{
				m_SetSelectedSection.Leave();
			}
		}

		/// <summary>
		/// Sets the button label.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="text"></param>
		public override void SetItemLabel(ushort index, string text)
		{
			m_SetTextSection.Enter();

			try
			{
				if (text == m_LabelCache.GetDefault(index, null))
					return;

				ushort key = (ushort)(START_TEXT_JOIN + index);
				SmartObject.SendInputSerial(key, text);

				m_LabelCache[index] = text;
			}
			finally
			{
				m_SetTextSection.Leave();
			}
		}

		/// <summary>
		/// Sets the button icon.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		public override void SetItemIcon(ushort index, string icon)
		{
			m_SetIconSection.Enter();

			try
			{
				if (icon == m_IconCache.GetDefault(index, null))
					return;

				ushort key = (ushort)(START_ICON_JOIN + index);
				SmartObject.SendInputSerial(key, icon);

				m_IconCache[index] = icon;
			}
			finally
			{
				m_SetIconSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribe to the smart object events.
		/// </summary>
		/// <param name="smartObject"></param>
		protected override void Subscribe(ISmartObject smartObject)
		{
			base.Subscribe(smartObject);

			smartObject.RegisterOutputSigChangeCallback(ITEM_CLICKED_JOIN, eSigType.Analog, ItemClickedCallback);
			smartObject.RegisterOutputSigChangeCallback(ITEM_HELD_JOIN, eSigType.Analog, ItemHeldCallback);
		}

		/// <summary>
		/// Unsubscribe from the smart object events.
		/// </summary>
		/// <param name="smartObject"></param>
		protected override void Unsubscribe(ISmartObject smartObject)
		{
			base.Unsubscribe(smartObject);

			smartObject.UnregisterOutputSigChangeCallback(ITEM_CLICKED_JOIN, eSigType.Analog, ItemClickedCallback);
			smartObject.UnregisterOutputSigChangeCallback(ITEM_HELD_JOIN, eSigType.Analog, ItemHeldCallback);
		}

		/// <summary>
		/// Called when an item in the list is clicked.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void ItemClickedCallback(object parent, SigAdapterEventArgs args)
		{
			Click((ushort)(args.Data.GetUShortValue() - 1));
		}

		/// <summary>
		/// Called when an item in the list is held.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void ItemHeldCallback(object parent, SigAdapterEventArgs args)
		{
			Hold((ushort)(args.Data.GetUShortValue() - 1));
		}

		#endregion
	}
}
