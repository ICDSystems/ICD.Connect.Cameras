namespace ICD.Connect.UI.PanelClients
{
	public abstract class AbstractSmartObjectClient : AbstractSigInputOutputClient, ISmartObjectClient
	{
		private readonly uint m_SmartObjectId;

		public uint SmartObjectId { get { return m_SmartObjectId; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="smartObjectId"></param>
		protected AbstractSmartObjectClient(uint smartObjectId)
		{
			m_SmartObjectId = smartObjectId;
		}
	}
}
