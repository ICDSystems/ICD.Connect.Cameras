using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsEventLogComponentView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_IndexLabel;
		private VtProSimpleLabel m_MessageLabel;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_IndexLabel = new VtProSimpleLabel(panel, m_Subpage);
			m_IndexLabel.SerialLabelJoins.Add(1);

			m_MessageLabel = new VtProSimpleLabel(panel, m_Subpage);
			m_MessageLabel.SerialLabelJoins.Add(2);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_IndexLabel;
			yield return m_MessageLabel;
		}
	}
}
