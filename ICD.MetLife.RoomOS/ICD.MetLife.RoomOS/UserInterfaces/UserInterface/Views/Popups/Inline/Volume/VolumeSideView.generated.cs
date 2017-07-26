﻿using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Guages;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Volume
{
	public sealed partial class VolumeSideView
	{
		private VtProSubpage m_Subpage;
		private VtProFormattedText m_Label;
		private VtProGuage m_Guage;
		private VtProButton m_VolumeUpButton;
		private VtProButton m_VolumeDownButton;
		private VtProButton m_MuteButton;

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
				DigitalVisibilityJoin = 22
			};

			m_Label = new VtProFormattedText(panel, m_Subpage);
			m_Label.SerialLabelJoins.Add(9);

			m_Guage = new VtProGuage(panel, m_Subpage)
			{
				AnalogFeedbackJoin = 9,
				DigitalEnableJoin = 27
			};

			m_VolumeUpButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 23
			};

			m_VolumeDownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 24
			};

			m_MuteButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 25
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Label;
			yield return m_Guage;
			yield return m_VolumeUpButton;
			yield return m_VolumeDownButton;
			yield return m_MuteButton;
		}
	}
}
