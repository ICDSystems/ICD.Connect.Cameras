using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Home
{
	public sealed partial class InACallView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_CallStatusList;

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
				DigitalVisibilityJoin = 89
			};

			m_CallStatusList = new VtProSubpageReferenceList(3, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 10,
				DigitalJoinIncrement = 1,
				SerialJoinIncrement = 2,
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
			yield return m_CallStatusList;
		}
	}
}
