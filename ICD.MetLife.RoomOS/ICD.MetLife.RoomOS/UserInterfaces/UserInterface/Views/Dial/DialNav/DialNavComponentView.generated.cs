using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial.DialNav
{
	public sealed partial class DialNavComponentView
	{
		private VtProSubpage m_Subpage;
		private VtProAdvancedButton m_Button;
		private VtProDynamicIconObject m_Icon;
		private VtProFormattedText m_Label;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_Button = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1
			};

			m_Icon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 2
			};

			m_Label = new VtProFormattedText(panel, m_Subpage);
			m_Label.SerialLabelJoins.Add(1);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Button;
			yield return m_Icon;
			yield return m_Label;
		}
	}
}
