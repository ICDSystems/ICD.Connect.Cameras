using ICD.Common.Properties;
using ICD.Connect.Panels;

namespace ICD.Connect.UI.Controls.TextControls
{
	public sealed class VtProFormattedText : AbstractVtProLabel
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		[PublicAPI]
		public VtProFormattedText(ISigInputOutput panel)
			: base(panel)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProFormattedText(ISigInputOutput panel, IVtProParent parent)
			: base(panel, parent)
		{
		}
	}
}
