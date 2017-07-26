using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsBaseView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_ExitButton;
		private VtProDynamicButtonList m_ButtonList;

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
				DigitalVisibilityJoin = 2100
			};

			m_ExitButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 69
			};

			m_ButtonList = new VtProDynamicButtonList(100, panel as IPanelDevice, m_Subpage)
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
			yield return m_ButtonList;
		}
	}
}
