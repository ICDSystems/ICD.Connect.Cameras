using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Panels.SmartObjectCollections;
using ICD.Connect.Panels.SmartObjects;

namespace ICD.Connect.Panels.Server
{
	public sealed class PanelServerSmartObjectCollection : ISmartObjectCollection
	{
		private readonly PanelServerDevice m_Device;

		private readonly Dictionary<uint, PanelServerSmartObject> m_SmartObjects;
		private readonly SafeCriticalSection m_SmartObjectsSection;

		/// <summary>
		/// Get the object at the specified number.
		/// </summary>
		/// <param name="paramKey">the key of the value to get.</param>
		/// <returns>
		/// Object stored at the key specified.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">Invalid Index Number specified.</exception>
		ISmartObject ISmartObjectCollection.this[uint paramKey] { get { return this[paramKey]; } }

		/// <summary>
		/// Get the object at the specified number.
		/// </summary>
		/// <param name="paramKey">the key of the value to get.</param>
		/// <returns>
		/// Object stored at the key specified.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">Invalid Index Number specified.</exception>
		public PanelServerSmartObject this[uint paramKey]
		{
			get
			{
				m_SmartObjectsSection.Enter();

				try
				{
					return LazyLoadSmartObject((ushort)paramKey);
				}
				finally
				{
					m_SmartObjectsSection.Leave();
				}
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		public PanelServerSmartObjectCollection(PanelServerDevice device)
		{
			m_SmartObjects = new Dictionary<uint, PanelServerSmartObject>();
			m_SmartObjectsSection = new SafeCriticalSection();

			m_Device = device;
		}

		#region Methods

		/// <summary>
		/// Clears the cached smart objects.
		/// </summary>
		public void Clear()
		{
			m_SmartObjectsSection.Enter();

			try
			{
				m_SmartObjects.Clear();
			}
			finally
			{
				m_SmartObjectsSection.Leave();
			}
		}

		public IEnumerator<KeyValuePair<uint, ISmartObject>> GetEnumerator()
		{
			m_SmartObjectsSection.Enter();

			try
			{
				return m_SmartObjects.Select(kvp => new KeyValuePair<uint, ISmartObject>(kvp.Key, kvp.Value))
				                     .ToList()
				                     .GetEnumerator();
			}
			finally
			{
				m_SmartObjectsSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Gets the smart object wrapper for the given key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private PanelServerSmartObject LazyLoadSmartObject(ushort key)
		{
			if (!m_SmartObjects.ContainsKey(key))
				m_SmartObjects[key] = new PanelServerSmartObject(m_Device, key);
			return m_SmartObjects[key];
		}

		#endregion
	}
}
