using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;
using ICD.Connect.UI.Widgets;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Meetings
{
	public sealed partial class ReserveNowView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_ReserveButton;
		private VtProButton m_CancelButton;
		private VtProFormattedText m_NextMeetingLabel;
		private SpinnerListClockWidget m_ClockWidget;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 76
			};

			m_ReserveButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 820,
				DigitalEnableJoin = 820
			};

			m_CancelButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 70,
			};

			m_NextMeetingLabel = new VtProFormattedText(panel, m_Subpage);
			m_NextMeetingLabel.SerialLabelJoins.Add(80);

			m_ClockWidget = new SpinnerListClockWidget(801, 802, 803, panel as IPanelDevice, m_Subpage);
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ReserveButton;
			yield return m_CancelButton;
			yield return m_NextMeetingLabel;
			yield return m_ClockWidget;
		}
	}
}
