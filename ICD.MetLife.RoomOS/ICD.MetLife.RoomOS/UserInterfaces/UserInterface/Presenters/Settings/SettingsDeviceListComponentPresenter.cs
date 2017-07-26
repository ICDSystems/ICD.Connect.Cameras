using System;
using ICD.Connect.Settings.Core;
using ICD.Connect.Settings;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsDeviceListComponentPresenter : AbstractComponentPresenter<ISettingsDeviceListComponentView>,
	                                                           ISettingsDeviceListComponentPresenter
	{
		public event EventHandler OnDeleteButtonPressed;
		public event EventHandler OnItemButtonPressed;

		private ISettings m_Settings;

		#region Properties

		/// <summary>
		/// Gets/sets the settings for this item.
		/// </summary>
		public ISettings Settings
		{
			get { return m_Settings; }
			set
			{
				if (value == m_Settings)
					return;

				m_Settings = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets the title label for this property.
		/// </summary>
		public string Title
		{
			get
			{
				if (m_Settings == null)
					return "New";

				string name = m_Settings.Name;
				name = string.IsNullOrEmpty(name) ? "Unnamed" : name;

				return string.Format("{0} - {1} ({2})", m_Settings.Id, name, m_Settings.FactoryName);
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
		public SettingsDeviceListComponentPresenter(int room, INavigationController nav,
		                                            IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDeleteButtonPressed = null;
			OnItemButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsDeviceListComponentView view)
		{
			base.Refresh(view);

			view.SetLabel(Title);
			view.ShowDeleteButton(m_Settings != null);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsDeviceListComponentView view)
		{
			base.Subscribe(view);

			view.OnDeleteButtonPressed += ViewOnDeleteButtonPressed;
			view.OnPressed += ViewOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsDeviceListComponentView view)
		{
			base.Unsubscribe(view);

			view.OnDeleteButtonPressed -= ViewOnDeleteButtonPressed;
			view.OnPressed -= ViewOnPressed;
		}

		/// <summary>
		/// Called when the user presses the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnPressed(object sender, EventArgs eventArgs)
		{
			OnItemButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the delete button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDeleteButtonPressed(object sender, EventArgs eventArgs)
		{
			OnDeleteButtonPressed.Raise(this);
		}

		#endregion
	}
}
