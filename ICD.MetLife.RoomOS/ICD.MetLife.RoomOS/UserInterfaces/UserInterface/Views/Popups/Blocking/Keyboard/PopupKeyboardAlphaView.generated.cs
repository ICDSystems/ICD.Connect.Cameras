using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Blocking.Keyboard
{
	public sealed partial class PopupKeyboardAlphaView
	{
		private VtProSubpage m_Subpage;

		private VtProButton m_SpecialButton;

		private VtProButton m_KeyQButton;
		private VtProButton m_KeyWButton;
		private VtProButton m_KeyEButton;
		private VtProButton m_KeyRButton;
		private VtProButton m_KeyTButton;
		private VtProButton m_KeyYButton;
		private VtProButton m_KeyUButton;
		private VtProButton m_KeyIButton;
		private VtProButton m_KeyOButton;
		private VtProButton m_KeyPButton;

		private VtProButton m_KeyAButton;
		private VtProButton m_KeySButton;
		private VtProButton m_KeyDButton;
		private VtProButton m_KeyFButton;
		private VtProButton m_KeyGButton;
		private VtProButton m_KeyHButton;
		private VtProButton m_KeyJButton;
		private VtProButton m_KeyKButton;
		private VtProButton m_KeyLButton;

		private VtProButton m_KeyZButton;
		private VtProButton m_KeyXButton;
		private VtProButton m_KeyCButton;
		private VtProButton m_KeyVButton;
		private VtProButton m_KeyBButton;
		private VtProButton m_KeyNButton;
		private VtProButton m_KeyMButton;

		private VtProButton m_KeySlashButton;
		private VtProButton m_KeyAtButton;
		private VtProButton m_KeyStopButton;

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
				DigitalVisibilityJoin = 2331,
				DigitalJoinOffset = 2000,
				SerialJoinOffset = 1000
			};

			m_SpecialButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 313
			};

			m_KeyQButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 713
			};
			m_KeyQButton.DigitalLabelJoins.Add(699);

			m_KeyWButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 714
			};
			m_KeyWButton.DigitalLabelJoins.Add(699);

			m_KeyEButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 715
			};
			m_KeyEButton.DigitalLabelJoins.Add(699);

			m_KeyRButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 716
			};
			m_KeyRButton.DigitalLabelJoins.Add(699);

			m_KeyTButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 717
			};
			m_KeyTButton.DigitalLabelJoins.Add(699);

			m_KeyYButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 718
			};
			m_KeyYButton.DigitalLabelJoins.Add(699);

			m_KeyUButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 719
			};
			m_KeyUButton.DigitalLabelJoins.Add(699);

			m_KeyIButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 720
			};
			m_KeyIButton.DigitalLabelJoins.Add(699);

			m_KeyOButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 721
			};
			m_KeyOButton.DigitalLabelJoins.Add(699);

			m_KeyPButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 722
			};
			m_KeyPButton.DigitalLabelJoins.Add(699);

			m_KeyAButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 725
			};
			m_KeyAButton.DigitalLabelJoins.Add(699);

			m_KeySButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 726
			};
			m_KeySButton.DigitalLabelJoins.Add(699);

			m_KeyDButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 727
			};
			m_KeyDButton.DigitalLabelJoins.Add(699);

			m_KeyFButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 728
			};
			m_KeyFButton.DigitalLabelJoins.Add(699);

			m_KeyGButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 729
			};
			m_KeyGButton.DigitalLabelJoins.Add(699);

			m_KeyHButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 730
			};
			m_KeyHButton.DigitalLabelJoins.Add(699);

			m_KeyJButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 731
			};
			m_KeyJButton.DigitalLabelJoins.Add(699);

			m_KeyKButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 732
			};
			m_KeyKButton.DigitalLabelJoins.Add(699);

			m_KeyLButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 733
			};
			m_KeyLButton.DigitalLabelJoins.Add(699);

			m_KeyZButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 737
			};
			m_KeyZButton.DigitalLabelJoins.Add(699);

			m_KeyXButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 738
			};
			m_KeyXButton.DigitalLabelJoins.Add(699);

			m_KeyCButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 739
			};
			m_KeyCButton.DigitalLabelJoins.Add(699);

			m_KeyVButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 740
			};
			m_KeyVButton.DigitalLabelJoins.Add(699);

			m_KeyBButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 741
			};
			m_KeyBButton.DigitalLabelJoins.Add(699);

			m_KeyNButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 742
			};
			m_KeyNButton.DigitalLabelJoins.Add(699);

			m_KeyMButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 743
			};
			m_KeyMButton.DigitalLabelJoins.Add(699);

			m_KeySlashButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 746
			};
			m_KeySlashButton.DigitalLabelJoins.Add(700);

			m_KeyAtButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 750
			};

			m_KeyStopButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 751
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;

			yield return m_SpecialButton;

			yield return m_KeyQButton;
			yield return m_KeyWButton;
			yield return m_KeyEButton;
			yield return m_KeyRButton;
			yield return m_KeyTButton;
			yield return m_KeyYButton;
			yield return m_KeyUButton;
			yield return m_KeyIButton;
			yield return m_KeyOButton;
			yield return m_KeyPButton;

			yield return m_KeyAButton;
			yield return m_KeySButton;
			yield return m_KeyDButton;
			yield return m_KeyFButton;
			yield return m_KeyGButton;
			yield return m_KeyHButton;
			yield return m_KeyJButton;
			yield return m_KeyKButton;
			yield return m_KeyLButton;

			yield return m_KeyZButton;
			yield return m_KeyXButton;
			yield return m_KeyCButton;
			yield return m_KeyVButton;
			yield return m_KeyBButton;
			yield return m_KeyNButton;
			yield return m_KeyMButton;

			yield return m_KeySlashButton;
			yield return m_KeyAtButton;
			yield return m_KeyStopButton;
		}
	}
}
