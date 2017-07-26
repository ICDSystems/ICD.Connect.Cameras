using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Home
{
	public sealed partial class RoomNumbersView
	{
		private VtProSubpage m_Subpage;
		private VtProFormattedText m_AudioText;
		private VtProFormattedText m_VideoText;
		private VtProFormattedText m_ShareStatusText;

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
				DigitalVisibilityJoin = 90
			};

			m_AudioText = new VtProFormattedText(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 8
			};
			m_AudioText.SerialLabelJoins.Add(7);
			
			m_VideoText = new VtProFormattedText(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 9
			};
			m_VideoText.SerialLabelJoins.Add(8);

			m_ShareStatusText = new VtProFormattedText(panel, m_Subpage);
			m_ShareStatusText.SerialLabelJoins.Add(15);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_AudioText;
			yield return m_VideoText;
			yield return m_ShareStatusText;
		}
	}
}
