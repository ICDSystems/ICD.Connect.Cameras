using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Lights
{
	public sealed partial class LightsView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_LightPresetsButtonList;
		private VtProSubpageReferenceList m_ShadesSubpageReferenceList;
		private VtProSubpageReferenceList m_LightsSubpageReferenceList;

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
				DigitalVisibilityJoin = 72
			};

			m_LightPresetsButtonList = new VtProDynamicButtonList(5, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 25
			};

			m_ShadesSubpageReferenceList = new VtProSubpageReferenceList(6, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 25,
				DigitalJoinIncrement = 3,
				SerialJoinIncrement = 1
			};

			m_LightsSubpageReferenceList = new VtProSubpageReferenceList(7, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 25,
				DigitalJoinIncrement = 2,
				SerialJoinIncrement = 1,
				AnalogJoinIncrement = 1
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_LightPresetsButtonList;
			yield return m_ShadesSubpageReferenceList;
			yield return m_LightsSubpageReferenceList;
		}
	}
}
