using ICD.Common.Properties;

namespace ICD.Connect.UI.PanelClients
{
	/// <summary>
	/// IPanelClient provides an interface for emulating a Crestron touch panel.
	/// </summary>
	public interface IPanelClient : ISigInputOutputClient
	{
		/// <summary>
		/// Collection containing the SmartObject clients.
		/// </summary>
		[PublicAPI]
		ISmartObjectClientCollection SmartObjects { get; }
	}
}
