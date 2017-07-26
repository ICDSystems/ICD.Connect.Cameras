using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TvTuner
{
	public sealed partial class TvTunerView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_ChannelUpButton;
		private VtProButton m_ChannelDownButton;
		private VtProButton m_StopWatchingTvButton;
		private VtProSubpageReferenceList m_ChannelList;

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
				DigitalVisibilityJoin = 92
			};

			m_ChannelUpButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 601
			};

			m_ChannelDownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 602
			};

			m_StopWatchingTvButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 600,
				DigitalEnableJoin = 600
			};

			m_ChannelList = new VtProSubpageReferenceList(21, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 50,
				DigitalJoinIncrement = 1,
				SerialJoinIncrement = 1
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ChannelUpButton;
			yield return m_ChannelDownButton;
			yield return m_StopWatchingTvButton;
			yield return m_ChannelList;
		}
	}
}
