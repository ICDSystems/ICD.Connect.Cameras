using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Common
{
	public sealed partial class StatusBarView
	{
		private VtProSubpage m_Subpage;
		private VtProImageObject m_BackgroundImage;
		private VtProButton m_SettingsButton;
		private VtProButton m_AudioButton;
		private VtProFormattedText m_RoomNameLabel;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_BackgroundImage = new VtProImageObject(panel, m_Subpage)
			{
				ModeAnalogJoin = 1
			};

			m_SettingsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 73
			};

			m_AudioButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 71
			};

			m_RoomNameLabel = new VtProFormattedText(panel, m_Subpage);
			m_RoomNameLabel.SerialLabelJoins.Add(1);
			m_RoomNameLabel.SerialLabelJoins.Add(2);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_BackgroundImage;
			yield return m_SettingsButton;
			yield return m_AudioButton;
			yield return m_RoomNameLabel;
		}
	}
}
