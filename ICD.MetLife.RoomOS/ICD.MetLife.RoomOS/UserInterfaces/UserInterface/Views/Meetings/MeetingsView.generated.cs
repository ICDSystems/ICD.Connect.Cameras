using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Meetings
{
	public sealed partial class MeetingsView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_ReserveNowButton;
		private VtProButton m_ReserveFutureButton;
		private VtProAdvancedButton m_CheckInButton;
		private VtProDynamicButtonList m_MeetingButtonList;
		private VtProFormattedText m_NoMeetingsLabel;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 98
			};

			m_ReserveNowButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 805,
				DigitalEnableJoin = 805
			};

			m_ReserveFutureButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 806,
				DigitalEnableJoin = 806
			};

			m_CheckInButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 804,
				DigitalEnableJoin = 803,
				AnalogModeJoin = 801
			};

			m_MeetingButtonList = new VtProDynamicButtonList(81, panel as IPanelDevice, m_Subpage)
			{
				DigitalVisibilityJoin = 800,
				MaxSize = 15
			};

			m_NoMeetingsLabel = new VtProFormattedText(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 801
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ReserveNowButton;
			yield return m_ReserveFutureButton;
			yield return m_CheckInButton;
			yield return m_MeetingButtonList;
			yield return m_NoMeetingsLabel;
		}
	}
}
