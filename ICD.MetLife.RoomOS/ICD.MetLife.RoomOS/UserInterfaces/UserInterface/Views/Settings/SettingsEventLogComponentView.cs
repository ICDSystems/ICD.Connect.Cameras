using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsEventLogComponentView : AbstractComponentView, ISettingsEventLogComponentView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public SettingsEventLogComponentView(ISigInputOutput panel, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
		}

		/// <summary>
		/// Sets the index label for the log item.
		/// </summary>
		/// <param name="index"></param>
		public void SetIndexLabel(uint index)
		{
			m_IndexLabel.SetLabelTextAtJoin(m_IndexLabel.SerialLabelJoins.First(), index.ToString());
		}

		/// <summary>
		/// Sets the message label for the log item.
		/// </summary>
		/// <param name="message"></param>
		public void SetMessageLabel(string message)
		{
			m_MessageLabel.SetLabelTextAtJoin(m_MessageLabel.SerialLabelJoins.First(), message);
		}
	}
}
