using ICD.Common.Properties;
using ICD.Connect.Panels;

namespace ICD.Connect.UI.Controls.Buttons
{
	public sealed class VtProButton : AbstractVtProButton
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		[PublicAPI]
		public VtProButton(ISigInputOutput panel)
			: base(panel)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProButton(ISigInputOutput panel, IVtProParent parent)
			: base(panel, parent)
		{
		}
	}
}
