using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsDeviceListView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_TitleLabel;
		private VtProButton m_ExitButton;
		private VtProSubpageReferenceList m_List;

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
				DigitalVisibilityJoin = 2101
			};

			m_TitleLabel = new VtProButton(panel, m_Subpage);
			m_TitleLabel.SerialLabelJoins.Add(1100);

			m_ExitButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2000
			};

			m_List = new VtProSubpageReferenceList(103, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 100,
				DigitalJoinIncrement = 3,
				SerialJoinIncrement = 1
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
			yield return m_ExitButton;
			yield return m_List;
		}
	}
}
