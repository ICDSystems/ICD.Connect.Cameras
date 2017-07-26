using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TvTuner.NavSource
{
	public sealed partial class GenericNavSourceView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_StopButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 93
			};

			m_StopButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 610,
				DigitalEnableJoin = 610
			};
			m_StopButton.SerialLabelJoins.Add(610);
			m_StopButton.SerialLabelJoins.Add(611);
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_StopButton;
		}
	}
}
