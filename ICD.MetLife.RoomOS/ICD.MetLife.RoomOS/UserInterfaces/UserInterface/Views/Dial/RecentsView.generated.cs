using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial
{
	public sealed partial class RecentsView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_RemoveButton;
		private VtProButton m_DialButton;
		private VtProSubpageReferenceList m_RecentCallsList;

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
				DigitalVisibilityJoin = 171
			};

			m_RemoveButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 331,
				DigitalVisibilityJoin = 331
			};

			m_DialButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 330,
				DigitalEnableJoin = 330
			};
			m_DialButton.SerialLabelJoins.Add(14);

			m_RecentCallsList = new VtProSubpageReferenceList(72, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 50,
				DigitalJoinIncrement = 2,
				AnalogJoinIncrement = 1,
				SerialJoinIncrement = 2
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_RemoveButton;
			yield return m_DialButton;
			yield return m_RecentCallsList;
		}
	}
}
