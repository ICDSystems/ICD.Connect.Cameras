namespace ICD.Connect.UI.PanelClients
{
	public abstract class AbstractPanelClient : AbstractSigInputOutputClient, IPanelClient
	{
		/// <summary>
		/// Collection containing the SmartObject clients.
		/// </summary>
		public abstract ISmartObjectClientCollection SmartObjects { get; }
	}
}
