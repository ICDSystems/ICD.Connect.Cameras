using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Common
{
	public sealed partial class ShutdownConfirmView
	{
		private VtProSubpage m_Subpage;
		private VtProFormattedText m_ShutdownMessage;
		private VtProButton m_CancelButton;
		private VtProButton m_ShutdownButton;

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
				DigitalVisibilityJoin = 65
			};

			m_ShutdownMessage = new VtProFormattedText(panel, m_Subpage);
			m_ShutdownMessage.AnalogLabelJoins.Add(2);

			m_CancelButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 67
			};

			m_ShutdownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 66
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ShutdownMessage;
			yield return m_CancelButton;
			yield return m_ShutdownButton;
		}
	}
}
