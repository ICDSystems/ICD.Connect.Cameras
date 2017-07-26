using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsDevicePropertiesComponentView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_PropertyLabel;
		private VtProButton m_PropertyButton;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_PropertyLabel = new VtProButton(panel, m_Subpage);
			m_PropertyLabel.SerialLabelJoins.Add(1);

			m_PropertyButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1
			};
			m_PropertyButton.SerialLabelJoins.Add(2);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_PropertyLabel;
			yield return m_PropertyButton;
		}
	}
}
