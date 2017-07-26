using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Home
{
	public sealed partial class CallStatusView
	{
		private VtProSubpage m_Subpage;
		private VtProFormattedText m_StatusText;
		private VtProButton m_EndCallButton;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_StatusText = new VtProFormattedText(panel, m_Subpage);
			m_StatusText.SerialLabelJoins.Add(1);
			m_StatusText.SerialLabelJoins.Add(2);
			m_StatusText.AnalogLabelJoins.Add(1);

			m_EndCallButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_StatusText;
			yield return m_EndCallButton;
		}
	}
}
