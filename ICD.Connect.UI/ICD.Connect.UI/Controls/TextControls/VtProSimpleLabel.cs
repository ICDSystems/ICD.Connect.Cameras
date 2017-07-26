using ICD.Common.Properties;
using ICD.Connect.Panels;

namespace ICD.Connect.UI.Controls.TextControls
{
	public sealed class VtProSimpleLabel : AbstractVtProLabel
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		[PublicAPI]
		public VtProSimpleLabel(ISigInputOutput panel)
			: base(panel)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProSimpleLabel(ISigInputOutput panel, IVtProParent parent)
			: base(panel, parent)
		{
		}
	}
}
