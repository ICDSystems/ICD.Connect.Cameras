using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Connect.Panels;

namespace ICD.Connect.UI.Controls.Lists
{
	public sealed class VtProSubpageReferenceList : AbstractVtProList, IVtProParent
	{
		private const ushort SCROLL_TO_ITEM_JOIN = 2;
		private const ushort DIGITAL_IS_MOVING_JOIN = 1;
		private const ushort NUMBER_OF_ITEMS_JOIN = 3;
		private const ushort ENABLE_START_JOIN = 11;
		private const ushort VISIBLE_START_JOIN = 2011;

		private readonly SafeCriticalSection m_EnabledSection;
		private readonly SafeCriticalSection m_VisibleSection;

		private readonly Dictionary<ushort, bool> m_VisibleItemsCache;
		private readonly Dictionary<ushort, bool> m_EnabledItemsCache;

		// Default values from VTPro environment.xml
		private ushort m_StartDigitalJoin = 4011;
		private ushort m_StartAnalogJoin = 11;
		private ushort m_StartSerialJoin = 11;

		#region Properties

		[PublicAPI]
		public ushort DigitalJoinIncrement { get; set; }

		[PublicAPI]
		public ushort AnalogJoinIncrement { get; set; }

		[PublicAPI]
		public ushort SerialJoinIncrement { get; set; }

		[PublicAPI]
		public ushort StartDigitalJoin { get { return m_StartDigitalJoin; } set { m_StartDigitalJoin = value; } }

		[PublicAPI]
		public ushort StartSerialJoin { get { return m_StartSerialJoin; } set { m_StartSerialJoin = value; } }

		[PublicAPI]
		public ushort StartAnalogJoin { get { return m_StartAnalogJoin; } set { m_StartAnalogJoin = value; } }

		[PublicAPI]
		public ushort StartItemVisibleJoin { get; set; }

		[PublicAPI]
		public ushort StartItemEnabledJoin { get; set; }

		protected override ushort AnalogNumberOfItemsJoin { get { return NUMBER_OF_ITEMS_JOIN; } }

		/// <summary>
		/// Gets the join number for scrolling to an item in the list.
		/// </summary>
		protected override ushort AnalogScrollToItemJoin { get { return SCROLL_TO_ITEM_JOIN; } }

		/// <summary>
		/// Gets the join number for getting the moving state of the list.
		/// </summary>
		protected override ushort DigitalIsMovingOutputJoin { get { return DIGITAL_IS_MOVING_JOIN; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		public VtProSubpageReferenceList(uint smartObjectId, IPanelDevice panel)
			: this(smartObjectId, panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProSubpageReferenceList(uint smartObjectId, IPanelDevice panel, IVtProParent parent)
			: base(smartObjectId, panel, parent)
		{
			m_VisibleSection = new SafeCriticalSection();
			m_EnabledSection = new SafeCriticalSection();

			m_VisibleItemsCache = new Dictionary<ushort, bool>();
			m_EnabledItemsCache = new Dictionary<ushort, bool>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sets the visible state for the item at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		public void SetItemVisible(ushort index, bool visible)
		{
			m_VisibleSection.Enter();

			try
			{
				if (visible == GetItemVisible(index))
					return;

				ushort key = (ushort)(VISIBLE_START_JOIN + index);
				SmartObject.SendInputDigital(key, visible);

				m_VisibleItemsCache[index] = visible;
			}
			finally
			{
				m_VisibleSection.Leave();
			}
		}

		/// <summary>
		/// Gets the visibility of the item at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool GetItemVisible(ushort index)
		{
			m_VisibleSection.Enter();

			try
			{
				return m_VisibleItemsCache.ContainsKey(index) && m_VisibleItemsCache[index];
			}
			finally
			{
				m_VisibleSection.Leave();
			}
		}

		/// <summary>
		/// Sets the enabled state for the item at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="enabled"></param>
		public void SetItemEnabled(ushort index, bool enabled)
		{
			m_EnabledSection.Enter();

			try
			{
				if (enabled == GetItemEnabled(index))
					return;

				ushort key = (ushort)(ENABLE_START_JOIN + index);
				SmartObject.SendInputDigital(key, enabled);

				m_EnabledItemsCache[index] = enabled;
			}
			finally
			{
				m_EnabledSection.Leave();
			}
		}

		/// <summary>
		/// Gets the enabled state of the item at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool GetItemEnabled(ushort index)
		{
			m_EnabledSection.Enter();

			try
			{
				return m_EnabledItemsCache.ContainsKey(index) && m_EnabledItemsCache[index];
			}
			finally
			{
				m_EnabledSection.Leave();
			}
		}

		#endregion

		#region IJoinOffsets Methods

		/// <summary>
		/// Gets the digital join offset for the given control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public ushort GetDigitalJoinOffset(IIndexed control)
		{
			ushort output = (ushort)(StartDigitalJoin - 1);
			output += (ushort)(control.Index * DigitalJoinIncrement);

			return output;
		}

		/// <summary>
		/// Gets the analog join offset for the given control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public ushort GetAnalogJoinOffset(IIndexed control)
		{
			ushort output = (ushort)(StartAnalogJoin - 1);
			output += (ushort)(control.Index * AnalogJoinIncrement);

			return output;
		}

		/// <summary>
		/// Gets the serial join offset for the given control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public ushort GetSerialJoinOffset(IIndexed control)
		{
			ushort output = (ushort)(StartSerialJoin - 1);
			output += (ushort)(control.Index * SerialJoinIncrement);

			return output;
		}

		#endregion
	}
}
