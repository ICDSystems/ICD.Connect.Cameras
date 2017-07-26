using System.Collections;
using System.Collections.Generic;
using ICD.Connect.Panels.SmartObjectCollections;
using ICD.Connect.Panels.SmartObjects;

namespace ICD.Connect.Panels.Mock
{
	public sealed class MockSmartObjectCollection : ISmartObjectCollection
	{
		private readonly Dictionary<uint, ISmartObject> m_SmartObjects;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MockSmartObjectCollection()
		{
			m_SmartObjects = new Dictionary<uint, ISmartObject>();
		}

		public IEnumerator<KeyValuePair<uint, ISmartObject>> GetEnumerator()
		{
			return m_SmartObjects.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Get the object at the specified number.
		/// 
		/// </summary>
		/// <param name="paramKey">the key of the value to get.</param>
		/// <returns>
		/// Object stored at the key specified.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">Invalid Index Number specified.</exception>
		public ISmartObject this[uint paramKey] { get { return m_SmartObjects[paramKey]; } }

		/// <summary>
		/// Clears the cached smart objects.
		/// </summary>
		public void Clear()
		{
			m_SmartObjects.Clear();
		}

		public void AddSmartObject(ISmartObject smartObject)
		{
			m_SmartObjects[smartObject.SmartObjectId] = smartObject;
		}
	}
}
