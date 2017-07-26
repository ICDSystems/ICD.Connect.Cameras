using ICD.Common.Properties;
using ICD.Connect.Panels;

namespace ICD.Connect.UI.Controls.Guages
{
	public sealed class VtProGuage : AbstractVtProGuage
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		[PublicAPI]
		public VtProGuage(ISigInputOutput panel)
			: base(panel)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProGuage(ISigInputOutput panel, IVtProParent parent)
			: base(panel, parent)
		{
		}
	}
}
