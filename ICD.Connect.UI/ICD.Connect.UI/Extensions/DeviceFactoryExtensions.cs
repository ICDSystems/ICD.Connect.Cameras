using ICD.Common.Properties;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.UI.Extensions
{
	public static class DeviceFactoryExtensions
	{
		[PublicAPI]
		public static IUserInterfaceFactory GetUiFactoryById(this IDeviceFactory factory, int id)
		{
			return factory.GetOriginatorById<IUserInterfaceFactory>(id);
		}
	}
}