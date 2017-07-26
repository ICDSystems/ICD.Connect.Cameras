using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Camera
{
	public sealed partial class CameraView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_CameraButtonList;
		private VtProDPad m_DPad;
		private VtProButton m_ZoomInButton;
		private VtProButton m_ZoomOutButton;
		private VtProButton m_SelfViewButton;
		private VtProButton m_SelfViewFullscreenButton;

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
				DigitalVisibilityJoin = 94
			};

			m_CameraButtonList = new VtProDynamicButtonList(41, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 16
			};

			m_DPad = new VtProDPad(42, panel as IPanelDevice, m_Subpage);

			m_ZoomInButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 451
			};

			m_ZoomOutButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 452
			};

			m_SelfViewButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 453
			};

			m_SelfViewFullscreenButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 454,
				DigitalVisibilityJoin = 455
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_CameraButtonList;
			yield return m_DPad;
			yield return m_ZoomInButton;
			yield return m_ZoomOutButton;
			yield return m_SelfViewButton;
			yield return m_SelfViewFullscreenButton;
		}
	}
}
