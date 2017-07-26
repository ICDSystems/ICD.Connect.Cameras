﻿using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views
{
	public sealed partial class SoundsView
	{
		private VtProSubpage m_Subpage;
		private VtProSound m_Ringtone;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_Ringtone = new VtProSound(panel as IPanelDevice)
			{
				JoinNumber = 11,
				StopSoundJoin = 12
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
		}
	}
}
