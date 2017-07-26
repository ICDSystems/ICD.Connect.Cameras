using System.Linq;
using ICD.Connect.Panels;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Common;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Common
{
	public sealed partial class PageMainView : AbstractView, IPageMainView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public PageMainView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Sets the title label text.
		/// </summary>
		/// <param name="title"></param>
		public void SetPageTitle(string title)
		{
			m_TitleLabel.SetLabelTextAtJoin(m_TitleLabel.SerialLabelJoins.First(), title);
		}

		#endregion
	}
}
