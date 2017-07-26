using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial
{
	public sealed partial class FavoritesView
	{
		private VtProSubpage m_Subpage;
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
				DigitalVisibilityJoin = 172
			};

			m_DialButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 315,
				DigitalEnableJoin = 315
			};
			m_DialButton.SerialLabelJoins.Add(12);

			m_FavoritesList = new VtProSubpageReferenceList(73, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 50,
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
			yield return m_DialButton;
			yield return m_FavoritesList;
		}
	}
}
