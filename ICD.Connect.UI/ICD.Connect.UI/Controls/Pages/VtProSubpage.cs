using ICD.Common.Properties;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls.Lists;

namespace ICD.Connect.UI.Controls.Pages
{
	public sealed class VtProSubpage : AbstractVtProPage<ISigInputOutput>
	{
		#region Properties

		/// <summary>
		/// Gets the visibility state of the control.
		/// </summary>
		public override bool IsVisible { get { return List == null ? base.IsVisible : List.GetItemVisible(Index); } }

		/// <summary>
		/// Gets the enabled state of the control.
		/// </summary>
		public override bool IsEnabled { get { return List == null ? base.IsEnabled : List.GetItemEnabled(Index); } }

		/// <summary>
		/// Returns the parent subpage reference list.
		/// </summary>
		/// <value></value>
		private VtProSubpageReferenceList List { get { return Parent as VtProSubpageReferenceList; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		[PublicAPI]
		public VtProSubpage(ISigInputOutput panel)
			: base(panel)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		[PublicAPI]
		public VtProSubpage(ISigInputOutput panel, IVtProParent parent)
			: base(panel, parent)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public VtProSubpage(ISigInputOutput panel, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Shows/hides the control. Throws InvalidOperationException if there is no visibility join.
		/// </summary>
		/// <param name="state"></param>
		public override void Show(bool state)
		{
			if (List == null)
				base.Show(state);
			else
				List.SetItemVisible(Index, state);
		}

		/// <summary>
		/// Enables/disables the button. Throws InvalidOperationException if there is no enable join.
		/// </summary>
		public override void Enable(bool state)
		{
			if (List == null)
				base.Enable(state);
			else
				List.SetItemEnabled(Index, state);
		}

		#endregion
	}
}
