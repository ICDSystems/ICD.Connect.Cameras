using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial
{
	public sealed partial class KeyboardSpecialView
	{
		private VtProSubpage m_Subpage;

		private VtProButton m_AlphabetButton;

		private VtProButton m_Key0Button;
		private VtProButton m_Key1Button;
		private VtProButton m_Key2Button;
		private VtProButton m_Key3Button;
		private VtProButton m_Key4Button;
		private VtProButton m_Key5Button;
		private VtProButton m_Key6Button;
		private VtProButton m_Key7Button;
		private VtProButton m_Key8Button;
		private VtProButton m_Key9Button;

		private VtProButton m_KeyDashButton;
		private VtProButton m_KeyEqualsButton;
		private VtProButton m_KeyBackslashButton;
		private VtProButton m_KeyColonButton;
		private VtProButton m_KeyApostrapheButton;
		private VtProButton m_KeyCommaButton;
		private VtProButton m_KeyPeriodButton;
		private VtProButton m_KeyOpenBracketButton;
		private VtProButton m_KeyCloseBracketButton;

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
				DigitalVisibilityJoin = 81
			};

			m_AlphabetButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 80
			};

			m_Key0Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 710
			};
			m_Key0Button.DigitalLabelJoins.Add(700);

			m_Key1Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 701
			};
			m_Key1Button.DigitalLabelJoins.Add(700);

			m_Key2Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 702
			};
			m_Key2Button.DigitalLabelJoins.Add(700);

			m_Key3Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 703
			};
			m_Key3Button.DigitalLabelJoins.Add(700);

			m_Key4Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 704
			};
			m_Key4Button.DigitalLabelJoins.Add(700);

			m_Key5Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 705
			};
			m_Key5Button.DigitalLabelJoins.Add(700);

			m_Key6Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 706
			};
			m_Key6Button.DigitalLabelJoins.Add(700);

			m_Key7Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 707
			};
			m_Key7Button.DigitalLabelJoins.Add(700);

			m_Key8Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 708
			};
			m_Key8Button.DigitalLabelJoins.Add(700);

			m_Key9Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 709
			};
			m_Key9Button.DigitalLabelJoins.Add(700);

			m_KeyDashButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 711
			};
			m_KeyDashButton.DigitalLabelJoins.Add(700);

			m_KeyEqualsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 712
			};
			m_KeyEqualsButton.DigitalLabelJoins.Add(700);

			m_KeyBackslashButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 723
			};
			m_KeyBackslashButton.DigitalLabelJoins.Add(700);
			 
			m_KeyColonButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 734
			};
			m_KeyColonButton.DigitalLabelJoins.Add(700);

			m_KeyApostrapheButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 735
			};
			m_KeyApostrapheButton.DigitalLabelJoins.Add(700);
		
			m_KeyCommaButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 744
			};
			m_KeyCommaButton.DigitalLabelJoins.Add(700);

			m_KeyPeriodButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 745
			};
			m_KeyPeriodButton.DigitalLabelJoins.Add(700);

			m_KeyOpenBracketButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 748
			};
			m_KeyOpenBracketButton.DigitalLabelJoins.Add(700);

			m_KeyCloseBracketButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 749
			};
			m_KeyCloseBracketButton.DigitalLabelJoins.Add(700);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_AlphabetButton;
			yield return m_Key0Button;
			yield return m_Key1Button;
			yield return m_Key2Button;
			yield return m_Key3Button;
			yield return m_Key4Button;
			yield return m_Key5Button;
			yield return m_Key6Button;
			yield return m_Key7Button;
			yield return m_Key8Button;
			yield return m_Key9Button;
			yield return m_KeyDashButton;
			yield return m_KeyEqualsButton;
			yield return m_KeyBackslashButton;
			yield return m_KeyColonButton;
			yield return m_KeyApostrapheButton;
			yield return m_KeyCommaButton;
			yield return m_KeyPeriodButton;
			yield return m_KeyOpenBracketButton;
			yield return m_KeyCloseBracketButton;
		}
	}
}
