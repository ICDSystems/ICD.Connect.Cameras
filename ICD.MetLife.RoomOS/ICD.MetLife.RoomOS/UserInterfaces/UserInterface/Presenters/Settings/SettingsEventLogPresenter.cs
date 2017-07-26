using System.Collections.Generic;
using System.Linq;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils;

using ICD.MetLife.RoomOS.Rooms;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsEventLogPresenter : AbstractPresenter<ISettingsEventLogView>, ISettingsEventLogPresenter
	{
		private readonly SettingsEventLogComponentPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsEventLogPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_ChildrenFactory = new SettingsEventLogComponentPresenterFactory(nav, ItemFactory);
			m_RefreshSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsEventLogView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				KeyValuePair<int, LogItem>[] settings = ServiceProvider.GetService<ILoggerService>()
				                                                       .GetHistory()
				                                                       .Reverse()
				                                                       .ToArray();

				foreach (ISettingsEventLogComponentPresenter presenter in m_ChildrenFactory.BuildChildren(settings))
					presenter.ShowView(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the given number of child views.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		private IEnumerable<ISettingsEventLogComponentView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(MetlifeRoom room)
		{
			base.Subscribe(room);

			ServiceProvider.GetService<ILoggerService>().OnEntryAdded += LoggingCoreOnEntryAdded;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(MetlifeRoom room)
		{
			base.Unsubscribe(room);

			ServiceProvider.GetService<ILoggerService>().OnEntryAdded -= LoggingCoreOnEntryAdded;
		}

		/// <summary>
		/// Called when a log is added to the logging core.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="logItemEventArgs"></param>
		private void LoggingCoreOnEntryAdded(object sender, LogItemEventArgs logItemEventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
