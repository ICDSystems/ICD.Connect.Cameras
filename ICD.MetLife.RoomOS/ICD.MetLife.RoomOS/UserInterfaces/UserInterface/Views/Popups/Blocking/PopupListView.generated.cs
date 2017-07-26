using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Blocking
{
	public sealed partial class PopupListView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_ExitButton;
		private VtProFormattedText m_TitleLabel;
		private VtProDynamicButtonList m_ItemList;

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
				DigitalVisibilityJoin = 2005
			};

			m_ExitButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2004
			};

			m_TitleLabel = new VtProFormattedText(panel, m_Subpage);
			m_TitleLabel.SerialLabelJoins.Add(1001);

			m_ItemList = new VtProDynamicButtonList(110, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 100
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ExitButton;
			yield return m_TitleLabel;
			yield return m_ItemList;
		}
	}
}
