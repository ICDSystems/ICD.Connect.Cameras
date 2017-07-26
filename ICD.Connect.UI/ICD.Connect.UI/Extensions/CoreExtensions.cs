using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.UI.Extensions
{
	public sealed class UserInterfaceFactoryCollection : AbstractOriginatorCollection<IUserInterfaceFactory>
	{
		public UserInterfaceFactoryCollection()
		{
		}

		public UserInterfaceFactoryCollection(IEnumerable<IUserInterfaceFactory> children) : base(children)
		{
		}
	}
	public static class CoreExtensions
	{
		public static UserInterfaceFactoryCollection GetUiFactories(this ICore core)
		{
			return new UserInterfaceFactoryCollection(core.Originators.OfType<IUserInterfaceFactory>());
		}
	}
}