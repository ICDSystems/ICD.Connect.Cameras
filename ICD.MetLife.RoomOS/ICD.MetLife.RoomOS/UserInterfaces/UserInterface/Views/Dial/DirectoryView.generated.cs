using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial
{
	public sealed partial class DirectoryView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_HomeButton;
		private VtProButton m_UpButton;
		private VtProButton m_DialButton;
		private VtProSubpageReferenceList m_FavoritesList;

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
				DigitalVisibilityJoin = 173
			};

			m_HomeButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 321,
				DigitalVisibilityJoin = 321
			};

			m_UpButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 322,
				DigitalVisibilityJoin = 322
			};

			m_DialButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 320,
				DigitalEnableJoin = 320
			};
			m_DialButton.SerialLabelJoins.Add(13);

			m_FavoritesList = new VtProSubpageReferenceList(74, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 500,
				DigitalJoinIncrement = 3,
				AnalogJoinIncrement = 1,
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
			yield return m_UpButton;
			yield return m_HomeButton;
			yield return m_DialButton;
			yield return m_FavoritesList;
		}
	}
}
