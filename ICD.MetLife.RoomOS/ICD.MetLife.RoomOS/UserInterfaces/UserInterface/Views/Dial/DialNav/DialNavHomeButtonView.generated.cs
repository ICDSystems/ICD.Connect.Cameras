﻿using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial.DialNav
{
	public sealed partial class DialNavHomeButtonView
	{
		private VtProSubpage m_Subpage;
		private VtProAdvancedButton m_Button;

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
				DigitalVisibilityJoin = 87
			};

			m_Button = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 90
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Button;
		}
	}
}
