using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.ShareMenu
{
	public sealed partial class ShareMenuView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_SourceList;
		private VtProButton m_StopSharingButton;

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
				DigitalVisibilityJoin = 91
			};

			m_SourceList = new VtProSubpageReferenceList(11, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 25,
				DigitalJoinIncrement = 1,
				SerialJoinIncrement = 3
			};

			m_StopSharingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 201,
				DigitalEnableJoin = 201
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_SourceList;
			yield return m_StopSharingButton;
		}
	}
}
