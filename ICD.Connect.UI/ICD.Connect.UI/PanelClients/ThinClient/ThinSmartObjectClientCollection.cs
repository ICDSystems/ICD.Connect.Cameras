namespace ICD.Connect.UI.PanelClients.ThinClient
{
	public sealed class ThinSmartObjectClientCollection : AbstractSmartObjectClientCollection
	{
		private readonly ThinPanelClient m_Parent;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		public ThinSmartObjectClientCollection(ThinPanelClient parent)
		{
			m_Parent = parent;
		}

		/// <summary>
		/// Creates a new ISmartObjectClient instance with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		protected override ISmartObjectClient InstantiateSmartObjectClient(uint id)
		{
			return new ThinSmartObjectClient(m_Parent, id);
		}
	}
}
