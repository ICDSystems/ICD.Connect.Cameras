using ICD.Common.Properties;

namespace ICD.Connect.Panels.SmartObjects
{
	public interface ISmartObject : ISigInputOutput
	{
		[PublicAPI]
		uint SmartObjectId { get; }
	}
}
