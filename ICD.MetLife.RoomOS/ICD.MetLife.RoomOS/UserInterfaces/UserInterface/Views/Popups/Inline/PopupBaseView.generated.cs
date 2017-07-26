using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline
{
	public sealed partial class PopupBaseView
	{
		private VtProSubpage m_Subpage;
		private VtProFormattedText m_TitleLabel;
		private VtProButton m_CloseButton;

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
				DigitalVisibilityJoin = 70
			};

			m_TitleLabel = new VtProFormattedText(panel, m_Subpage);
			m_TitleLabel.SerialLabelJoins.Add(6);

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 70
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_TitleLabel;
			yield return m_CloseButton;
		}
	}
}
