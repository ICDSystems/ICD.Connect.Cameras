using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Panels.Extensions
{
	public sealed class CorePanelCollection : AbstractOriginatorCollection<IPanelDevice>
	{
		public CorePanelCollection()
		{	
		}

		public CorePanelCollection(IEnumerable<IPanelDevice> children) : base(children)
		{
		}
	}
	public static class CoreExtensions
	{
		public static CorePanelCollection GetPanels(this ICore core)
		{
			return new CorePanelCollection(core.Originators.OfType<IPanelDevice>());
		}
	}
}