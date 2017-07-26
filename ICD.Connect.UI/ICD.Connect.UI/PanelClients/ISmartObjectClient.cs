using ICD.Common.Properties;

namespace ICD.Connect.UI.PanelClients
{
	public interface ISmartObjectClient : ISigInputOutputClient
	{
		[PublicAPI]
		uint SmartObjectId { get; }
	}
}
