namespace ICD.Connect.UI.PanelClients
{
	public interface ISmartObjectClientCollection
	{
		/// <summary>
		/// Get the object at the specified number.
		/// 
		/// </summary>
		/// <param name="paramKey">the key of the value to get.</param>
		/// <returns>
		/// Object stored at the key specified.
		/// </returns>
		/// <exception cref="T:System.IndexOutOfRangeException">Invalid Index Number specified.</exception>
		ISmartObjectClient this[uint paramKey] { get; }

		/// <summary>
		/// Clears the cached smart objects.
		/// </summary>
		void Clear();
	}
}