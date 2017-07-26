using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TouchTones
{
	public sealed partial class TouchTonesView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_ContactList;
		private VtProSimpleKeypad m_Keypad;

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
				DigitalVisibilityJoin = 96
			};

			m_ContactList = new VtProDynamicButtonList(61, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 10,
				DigitalVisibilityJoin = 350
			};

			m_Keypad = new VtProSimpleKeypad(62, panel as IPanelDevice, m_Subpage);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ContactList;
			yield return m_Keypad;
		}
	}
}
