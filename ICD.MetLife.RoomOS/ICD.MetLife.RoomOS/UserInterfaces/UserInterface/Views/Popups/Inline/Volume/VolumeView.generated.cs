using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Volume
{
	public sealed partial class VolumeView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_VolumeComponentsList;

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
				DigitalVisibilityJoin = 71
			};

			m_VolumeComponentsList = new VtProSubpageReferenceList(2, panel as IPanelDevice, m_Subpage)
			{
				DigitalJoinIncrement = 5,
				AnalogJoinIncrement = 1,
				SerialJoinIncrement = 1,
				MaxSize = 10
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_VolumeComponentsList;
		}
	}
}
