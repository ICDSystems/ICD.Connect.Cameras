﻿using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial
{
	public sealed partial class RecentCallView
	{
		private VtProSubpage m_Subpage;
		private VtProAdvancedButton m_IconButton;
		private VtProButton m_BackgroundButton;
		private VtProFormattedText m_FormattedText;
		private VtProButton m_FavoriteButton;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_IconButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				AnalogModeJoin = 1
			};

			m_BackgroundButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1
			};

			m_FormattedText = new VtProFormattedText(panel, m_Subpage);
			m_FormattedText.SerialLabelJoins.Add(1);
			m_FormattedText.SerialLabelJoins.Add(2);

			m_FavoriteButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_IconButton;
			yield return m_BackgroundButton;
			yield return m_FormattedText;
			yield return m_FavoriteButton;
		}
	}
}
