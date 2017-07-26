using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Panels.SmartObjects;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.UI.Controls.Lists
{
	[PublicAPI]
	public sealed class VtProSpinnerList : AbstractVtProSmartObject
	{
		// Bool input
		//private const ushort DIGITAL_DISABLE_REDRAW_JOIN = 1;
		//private const ushort DIGITAL_DISABLE_SCROLL_ANIMATION_JOIN = 2;
		//private const ushort DIGITAL_ENDLESS_SCROLLING_JOIN = 3;
		private const ushort DIGITAL_NEXT_ITEM_JOIN = 4;
		private const ushort DIGITAL_PREVIOUS_ITEM_JOIN = 5;
		private const ushort DIGITAL_SET_ITEM_VISIBLE_START = 2011;

		// Bool output
		//private const ushort DIGITAL_FORWARD_OVERFLOW = 4;
		//private const ushort DIGITAL_REVERSE_OVERFLOW = 5;

		// Ushort input
		private const ushort ANALOG_SELECT_ITEM_JOIN = 2;
		private const ushort ANALOG_SET_NUMBER_OF_ITEMS_JOIN = 3;
		//private const ushort ANALOG_MASK_MODE_JOIN = 4;

		// Ushort output
		private const ushort ANALOG_ITEM_SELECTED_JOIN = 1;

		// Serial input
		private const ushort SERIAL_SET_ITEM_TEXT_START = 11;

		[PublicAPI]
		public event EventHandler<UShortEventArgs> OnItemSelected;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProSpinnerList(uint smartObjectId, IPanelDevice panel, IVtProParent parent)
			: base(smartObjectId, panel, parent)
		{
		}

		#region Methods

		/// <summary>
		/// Sets the number of items in the list.
		/// </summary>
		/// <param name="count"></param>
		[PublicAPI]
		public void SetNumberOfItems(ushort count)
		{
			SmartObject.SendInputAnalog(ANALOG_SET_NUMBER_OF_ITEMS_JOIN, count);
		}

		/// <summary>
		/// Selects the item at the given index.
		/// </summary>
		/// <param name="item"></param>
		[PublicAPI]
		public void SelectItem(ushort item)
		{
			SmartObject.SendInputAnalog(ANALOG_SELECT_ITEM_JOIN, (ushort)(item + 1));
		}

		/// <summary>
		/// Scrolls to the next item.
		/// </summary>
		[PublicAPI]
		public void NextItem()
		{
			SmartObject.SendInputDigital(DIGITAL_NEXT_ITEM_JOIN, true);
		}

		/// <summary>
		/// Scrolls to the previous item
		/// </summary>
		[PublicAPI]
		public void PreviousItem()
		{
			SmartObject.SendInputDigital(DIGITAL_PREVIOUS_ITEM_JOIN, true);
		}

		/// <summary>
		/// Sets the visibility of the item at the given index.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="visible"></param>
		[PublicAPI]
		public void SetItemVisible(ushort item, bool visible)
		{
			SmartObject.SendInputDigital((ushort)(DIGITAL_SET_ITEM_VISIBLE_START + item), visible);
		}

		/// <summary>
		/// Sets the label for the item at the given index.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="label"></param>
		[PublicAPI]
		public void SetItemLabel(ushort item, string label)
		{
			SmartObject.SendInputSerial((ushort)(SERIAL_SET_ITEM_TEXT_START + item), label);
		}

		/// <summary>
		/// Sets the number of items and the item labels.
		/// </summary>
		/// <param name="labels"></param>
		[PublicAPI]
		public void SetItemLabels(string[] labels)
		{
			SetNumberOfItems((ushort)labels.Length);

			for (ushort index = 0; index < labels.Length; index++)
				SetItemLabel(index, labels[index]);
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

			smartObject.RegisterOutputSigChangeCallback(ANALOG_ITEM_SELECTED_JOIN, eSigType.Analog, ItemSelectedCallback);
		}

		/// <summary>
		/// Unsubscribe from the smart object events.
		/// </summary>
		/// <param name="smartObject"></param>
		protected override void Unsubscribe(ISmartObject smartObject)
		{
			base.Unsubscribe(smartObject);

			smartObject.UnregisterOutputSigChangeCallback(ANALOG_ITEM_SELECTED_JOIN, eSigType.Analog, ItemSelectedCallback);
		}

		/// <summary>
		/// Called when an item in the list is clicked.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void ItemSelectedCallback(object parent, SigAdapterEventArgs args)
		{
			OnItemSelected.Raise(this, new UShortEventArgs((ushort)(args.Data.GetUShortValue() - 1)));
		}

		#endregion
	}
}
