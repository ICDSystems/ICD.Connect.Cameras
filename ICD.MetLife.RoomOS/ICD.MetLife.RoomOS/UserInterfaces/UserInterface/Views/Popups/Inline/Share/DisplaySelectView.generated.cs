using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Share
{
	public sealed partial class DisplaySelectView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_ListenToSourceButton;
		private VtProButton m_ShowVideoCallButton;
		private VtProFormattedText m_ShareSourceLabel;
		private VtProDynamicButtonList m_DestinationList;

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
				DigitalVisibilityJoin = 77
			};

			m_ListenToSourceButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 120,
				DigitalEnableJoin = 121
			};

			m_ShowVideoCallButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 122,
				DigitalVisibilityJoin = 123,
				DigitalEnableJoin = 124
			};

			m_ShareSourceLabel = new VtProFormattedText(panel, m_Subpage);
			m_ShareSourceLabel.SerialLabelJoins.Add(30);

			m_DestinationList = new VtProDynamicButtonList(12, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 50
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ListenToSourceButton;
			yield return m_ShowVideoCallButton;
			yield return m_ShareSourceLabel;
			yield return m_DestinationList;
		}
	}
}
