using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Blocking
{
	public sealed partial class AlertBoxView
	{
		private VtProSubpage m_Subpage;
		private VtProFormattedText m_MessageText;
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
				DigitalVisibilityJoin = 28
			};

			m_MessageText = new VtProFormattedText(panel, m_Subpage);
			m_MessageText.SerialLabelJoins.Add(16);

			m_ButtonList = new VtProDynamicButtonList(8, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 2
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_MessageText;
			yield return m_ButtonList;
		}
	}
}
