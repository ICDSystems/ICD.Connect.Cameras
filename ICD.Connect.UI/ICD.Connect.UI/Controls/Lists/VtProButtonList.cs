using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Panels.SmartObjects;

namespace ICD.Connect.UI.Controls.Lists
{
	[PublicAPI]
	public sealed class VtProButtonList : AbstractVtProButtonList
	{
		public event EventHandler<UShortEventArgs> OnButtonPressed;
		public event EventHandler<UShortEventArgs> OnButtonReleased;

		private readonly SafeCriticalSection m_SetItemLabelSection;
		private readonly SafeCriticalSection m_SetItemIconSection;
		private readonly SafeCriticalSection m_SetItemSelectedSection;

		protected override ushort AnalogNumberOfItemsJoin { get { throw new NotImplementedException(); } }

		/// <summary>
		/// Gets the join number for scrolling to an item in the list.
		/// </summary>
		protected override ushort AnalogScrollToItemJoin { get { throw new NotImplementedException(); } }

		/// <summary>
		/// Gets the join number for getting the moving state of the list.
		/// </summary>
		protected override ushort DigitalIsMovingOutputJoin { get { throw new NotImplementedException(); } }

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		public VtProButtonList(uint smartObjectId, IPanelDevice panel)
			: this(smartObjectId, panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProButtonList(uint smartObjectId, IPanelDevice panel, IVtProParent parent)
			: base(smartObjectId, panel, parent)
		{
			m_SetItemLabelSection = new SafeCriticalSection();
			m_SetItemIconSection = new SafeCriticalSection();
			m_SetItemSelectedSection = new SafeCriticalSection();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnButtonPressed = null;
			OnButtonReleased = null;

			base.Dispose();
		}

		/// <summary>
		/// Simulates a Press on the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		[PublicAPI]
		public void Press(ushort index)
		{
			if (IsVisibleRecursive)
				OnButtonPressed.Raise(this, new UShortEventArgs(index));
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
		/// Sets the selected state for the button at the given index.
		/// todo - caching
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		public override void SetItemSelected(ushort index, bool selected)
		{
			m_SetItemSelectedSection.Enter();

			try
			{
				throw new NotImplementedException();
				//string key = selected ? "Select Item" : "Deselect Item";
				//SmartObject.SendInputAnalog(key, (ushort)(index + 1));
			}
			finally
			{
				m_SetItemSelectedSection.Leave();
			}
		}

		/// <summary>
		/// Sets the button label.
		/// todo - caching
		/// </summary>
		/// <param name="index"></param>
		/// <param name="text"></param>
		public override void SetItemLabel(ushort index, string text)
		{
			m_SetItemLabelSection.Enter();

			try
			{
				throw new NotImplementedException();
				//string key = string.Format("Item {0} Text", index + 1);
				//SmartObject.SendInputSerial(key, text);
			}
			finally
			{
				m_SetItemLabelSection.Leave();
			}
		}

		/// <summary>
		/// Sets the button icon.
		/// todo - caching
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		public override void SetItemIcon(ushort index, string icon)
		{
			m_SetItemIconSection.Enter();

			try
			{
				throw new NotImplementedException();
				//string key = string.Format("Item {0} Icon Serial", index + 1);
				//SmartObject.SendInputSerial(key, icon);
			}
			finally
			{
				m_SetItemIconSection.Leave();
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

			throw new NotImplementedException();
			//smartObject.RegisterOutputSigChangeCallback("Item Clicked", eSigType.UShort, ItemClicked);
		}

		/// <summary>
		/// Unsubscribe from the smart object events.
		/// </summary>
		/// <param name="smartObject"></param>
		protected override void Unsubscribe(ISmartObject smartObject)
		{
			base.Unsubscribe(smartObject);

			throw new NotImplementedException();
			//smartObject.UnregisterOutputSigChangeCallback("Item Clicked", eSigType.UShort, ItemClicked);
		}

		/// <summary>
		/// Called when an item in the list is clicked.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void ItemClicked(object parent, SigAdapterEventArgs args)
		{
			ushort index = (ushort)(args.Data.GetUShortValue() - 1);

			if (args.Data.GetBoolValue())
				Press(index);
			else
				Release(index);
		}

		#endregion
	}
}
