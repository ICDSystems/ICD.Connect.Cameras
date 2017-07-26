using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Lights
{
	public sealed partial class ShadeComponentView
	{
		private VtProSubpage m_Subpage;
		private VtProFormattedText m_Title;
		private VtProButton m_DownButton;
		private VtProButton m_UpButton;
		private VtProButton m_StopButton;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_Title = new VtProFormattedText(panel, m_Subpage);
			m_Title.SerialLabelJoins.Add(1);

			m_DownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2
			};

			m_StopButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3
			};

			m_UpButton = new VtProButton(panel, m_Subpage)
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
			yield return m_Title;
			yield return m_DownButton;
			yield return m_UpButton;
			yield return m_StopButton;
		}
	}
}
