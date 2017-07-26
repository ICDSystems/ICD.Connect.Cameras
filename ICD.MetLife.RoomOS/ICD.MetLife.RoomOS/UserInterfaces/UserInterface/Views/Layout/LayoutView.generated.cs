using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Layout
{
	public sealed partial class LayoutView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_AutoButton;
		private VtProButton m_EqualButton;
		private VtProButton m_ProminentButton;
		private VtProButton m_OverlayButton;
		private VtProButton m_SingleButton;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 95
			};

			m_AutoButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 401
			};

			m_EqualButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 402
			};

			m_ProminentButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 403
			};

			m_OverlayButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 404
			};

			m_SingleButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 405
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_AutoButton;
			yield return m_EqualButton;
			yield return m_ProminentButton;
			yield return m_OverlayButton;
			yield return m_SingleButton;
		}
	}
}
