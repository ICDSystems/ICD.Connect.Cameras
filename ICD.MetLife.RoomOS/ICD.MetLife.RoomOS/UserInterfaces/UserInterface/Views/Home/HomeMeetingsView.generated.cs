using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Home
{
	public sealed partial class HomeMeetingsView
	{
		private VtProSubpage m_Subpage;
		private VtProAdvancedButton m_CheckInButton;
		private VtProFormattedText m_CurrentMeetingText;
		private VtProFormattedText m_NextMeetingText;

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
				DigitalVisibilityJoin = 86
			};

			m_CheckInButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 834,
				DigitalEnableJoin = 833,
				AnalogModeJoin = 800
			};

			m_CurrentMeetingText = new VtProFormattedText(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 830
			};
			m_CurrentMeetingText.SerialLabelJoins.Add(81);

			m_NextMeetingText = new VtProFormattedText(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 831,
			};
			m_NextMeetingText.SerialLabelJoins.Add(82);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_CheckInButton;
			yield return m_CurrentMeetingText;
			yield return m_NextMeetingText;
		}
	}
}
