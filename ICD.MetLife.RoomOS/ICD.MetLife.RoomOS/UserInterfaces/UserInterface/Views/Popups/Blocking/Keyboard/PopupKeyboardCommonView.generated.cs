using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Blocking.Keyboard
{
	public sealed partial class PopupKeyboardCommonView
	{
		private VtProSubpage m_Subpage;
		private VtProTextEntry m_TextEntry;
		private VtProButton m_BackspaceButton;
		private VtProButton m_ClearButton;
		private VtProButton m_ShiftButton;
		private VtProButton m_CapsButton;
		private VtProButton m_SpaceButton;
		private VtProButton m_SubmitButton;
		private VtProButton m_CancelButton;

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
				DigitalVisibilityJoin = 2330,
				DigitalJoinOffset = 2000,
				SerialJoinOffset = 1000
			};

			m_TextEntry = new VtProTextEntry(panel, m_Subpage)
			{
				SerialOutputJoin = 10
			};
			m_TextEntry.SerialLabelJoins.Add(10);

			m_BackspaceButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 301,
				DigitalEnableJoin = 301
			};

			m_ClearButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 302,
				DigitalEnableJoin = 302
			};

			m_ShiftButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 736
			};

			m_CapsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 724
			};

			m_SpaceButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 747
			};

			m_SubmitButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 316,
				DigitalEnableJoin = 301
			};
			m_SubmitButton.SerialLabelJoins.Add(28);

			m_CancelButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 317
			};
			m_CancelButton.SerialLabelJoins.Add(29);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_TextEntry;
			yield return m_BackspaceButton;
			yield return m_ClearButton;
			yield return m_ShiftButton;
			yield return m_CapsButton;
			yield return m_SpaceButton;
			yield return m_SubmitButton;
			yield return m_CancelButton;
		}
	}
}
