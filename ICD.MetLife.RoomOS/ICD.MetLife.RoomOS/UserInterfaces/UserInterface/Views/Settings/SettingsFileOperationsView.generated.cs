using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsFileOperationsView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_PanelSetupButton;
		private VtProButton m_ProgramResetButton;
		private VtProButton m_ProcessorResetButton;
		private VtProButton m_ApplyButton;
		private VtProButton m_RevertButton;
		private VtProButton m_LoadButton;
		private VtProFormattedText m_ProgramInfoText;

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
				DigitalVisibilityJoin = 2105
			};

			m_PanelSetupButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 17242
			};

			m_ProgramResetButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2010
			};

			m_ProcessorResetButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2011
			};

			m_ApplyButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2012
			};

			m_RevertButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2013
			};

			m_LoadButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2014
			};

			m_ProgramInfoText = new VtProFormattedText(panel, m_Subpage);
			m_ProgramInfoText.SerialLabelJoins.Add(1005);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_PanelSetupButton;
			yield return m_ProgramResetButton;
			yield return m_ProcessorResetButton;
			yield return m_ApplyButton;
			yield return m_RevertButton;
			yield return m_LoadButton;
			yield return m_ProgramInfoText;
		}
	}
}
