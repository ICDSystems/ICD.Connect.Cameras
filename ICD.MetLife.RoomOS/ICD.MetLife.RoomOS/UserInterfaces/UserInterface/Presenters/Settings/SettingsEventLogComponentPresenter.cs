using ICD.Common.Services.Logging;
using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsEventLogComponentPresenter : AbstractComponentPresenter<ISettingsEventLogComponentView>,
	                                                         ISettingsEventLogComponentPresenter
	{
		private LogItem m_LogItem;
		private uint m_Index;

		#region Properties

		/// <summary>
		/// Gets/sets the log item.
		/// </summary>
		public LogItem LogItem
		{
			get { return m_LogItem; }
			set
			{
				if (value == m_LogItem)
					return;

				m_LogItem = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets/sets the index.
		/// </summary>
		public uint Index
		{
			get { return m_Index; }
			set
			{
				if (value == m_Index)
					return;

				m_Index = value;

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsEventLogComponentPresenter(int room, INavigationController nav, IViewFactory views,
		                                          ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsEventLogComponentView view)
		{
			base.Refresh(view);

			string message = string.Format("{0} - {1} - {2}", m_LogItem.Timestamp, m_LogItem.Severity, m_LogItem.Message);

			view.SetMessageLabel(message);
			view.SetIndexLabel(m_Index);
		}
	}
}
