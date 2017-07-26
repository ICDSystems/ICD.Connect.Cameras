using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Blocking
{
	public sealed partial class IncomingCallView
	{
		private VtProSubpage m_Subpage;
		private VtProFormattedText m_MessageLabel;
		private VtProButton m_RejectButton;
		private VtProButton m_CancelButton;
		private VtProButton m_AcceptButton;

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
				DigitalVisibilityJoin = 19
			};

			m_MessageLabel = new VtProFormattedText(panel, m_Subpage);
			m_MessageLabel.SerialLabelJoins.Add(18);

			m_RejectButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2200,
				DigitalVisibilityJoin = 2200
			};

			m_CancelButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2201,
				DigitalVisibilityJoin = 2201
			};

			m_AcceptButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2202,
				DigitalVisibilityJoin = 2202
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_MessageLabel;
			yield return m_RejectButton;
			yield return m_CancelButton;
			yield return m_AcceptButton;
		}
	}
}
