using System.Collections.Generic;
using ICD.Common.Utils;

namespace ICD.Connect.UI.PanelClients
{
	public abstract class AbstractSmartObjectClientCollection : ISmartObjectClientCollection
	{
		private readonly Dictionary<uint, ISmartObjectClient> m_SmartObjects;
		private readonly SafeCriticalSection m_SmartObjectsSection;

		/// <summary>
		/// Get the object at the specified number.
		/// </summary>
		/// <param name="paramKey">the key of the value to get.</param>
		/// <returns>
		/// Object stored at the key specified.
		/// </returns>
		public ISmartObjectClient this[uint paramKey]
		{
			get
			{
				m_SmartObjectsSection.Enter();

				try
				{
					if (!m_SmartObjects.ContainsKey(paramKey))
						m_SmartObjects[paramKey] = InstantiateSmartObjectClient(paramKey);
					return m_SmartObjects[paramKey];
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
		protected AbstractSmartObjectClientCollection()
		{
			m_SmartObjects = new Dictionary<uint, ISmartObjectClient>();
			m_SmartObjectsSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Clears the cached smart objects.
		/// </summary>
		public void Clear()
		{
			m_SmartObjectsSection.Execute(() => m_SmartObjects.Clear());
		}

		/// <summary>
		/// Creates a new ISmartObjectClient instance with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		protected abstract ISmartObjectClient InstantiateSmartObjectClient(uint id);
	}
}
