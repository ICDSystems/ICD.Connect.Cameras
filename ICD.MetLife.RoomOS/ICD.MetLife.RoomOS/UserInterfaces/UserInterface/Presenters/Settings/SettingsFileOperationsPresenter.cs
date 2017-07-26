using System;
using System.Text.RegularExpressions;
using ICD.Common.Properties;
using ICD.Connect.Krang.Settings;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Popups.Blocking;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Settings;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Settings
{
	public sealed class SettingsFileOperationsPresenter : AbstractPresenter<ISettingsFileOperationsView>,
	                                                      ISettingsFileOperationsPresenter
	{
		private IAlertBoxPresenter m_AlertBox;

		[UsedImplicitly] private object m_ApplySettingsHandle;

		[UsedImplicitly] private object m_LoadHandle;

		/// <summary>
		/// Gets the alert box menu.
		/// </summary>
		private IAlertBoxPresenter AlertBox
		{
			get { return m_AlertBox ?? (m_AlertBox = Navigation.LazyLoadPresenter<IAlertBoxPresenter>()); }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public SettingsFileOperationsPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsFileOperationsView view)
		{
			base.Refresh(view);

			string progInfo = string.Empty;
			string command = string.Format("progcomments:{0}", ProgramUtils.ProgramNumber);
			IcdConsole.SendControlSystemCommand(command, ref progInfo);

			// Remove duplicate spaces
			Regex regex = new Regex("[ ]{2,}");
			progInfo = regex.Replace(progInfo, " ");

			view.SetProgramInfoText(progInfo);
		}

		#region Private Methods

		/// <summary>
		/// Saves settings and applies them to the room.
		/// </summary>
		private void ApplySettings()
		{
			ICoreSettings settings = Navigation.LazyLoadPresenter<ISettingsBasePresenter>().SettingsInstance;

			try
			{
				FileOperations.SaveSettings(settings);
			}
			catch (Exception e)
			{
				AlertBox.Enqueue("Error saving settings", e.Message, new AlertOption("Close"));
				return;
			}

			Core.ApplySettings(settings);
		}

		/// <summary>
		/// Loads the settings from disk to the room.
		/// </summary>
		private void LoadSettings()
		{
			Core.LoadSettings();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsFileOperationsView view)
		{
			base.Subscribe(view);

			view.OnLoadButtonPressed += ViewOnLoadButtonPressed;
			view.OnPanelSetupButtonPressed += ViewOnPanelSetupButtonPressed;
			view.OnProcessorResetButtonPressed += ViewOnProcessorResetButtonPressed;
			view.OnProgramResetButtonPressed += ViewOnProgramResetButtonPressed;
			view.OnSaveAndRunButtonPressed += ViewOnSaveAndRunButtonPressed;
			view.OnUndoButtonPressed += ViewOnUndoButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsFileOperationsView view)
		{
			base.Unsubscribe(view);

			view.OnLoadButtonPressed -= ViewOnLoadButtonPressed;
			view.OnPanelSetupButtonPressed -= ViewOnPanelSetupButtonPressed;
			view.OnProcessorResetButtonPressed -= ViewOnProcessorResetButtonPressed;
			view.OnProgramResetButtonPressed -= ViewOnProgramResetButtonPressed;
			view.OnSaveAndRunButtonPressed -= ViewOnSaveAndRunButtonPressed;
			view.OnUndoButtonPressed -= ViewOnUndoButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the undo button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnUndoButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.LazyLoadPresenter<ISettingsBasePresenter>().RevertSettingsInstance();
		}

		/// <summary>
		/// Called when the user presses the undo button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSaveAndRunButtonPressed(object sender, EventArgs eventArgs)
		{
			// Apply settings in a new thread to avoid disposing this panel in the UI thread.
			m_ApplySettingsHandle = CrestronUtils.SafeInvoke(ApplySettings);
		}

		/// <summary>
		/// Called when the user presses the program reset button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnProgramResetButtonPressed(object sender, EventArgs eventArgs)
		{
			CrestronUtils.RestartProgram();
		}

		/// <summary>
		/// Called when the user presses the processor reset button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnProcessorResetButtonPressed(object sender, EventArgs eventArgs)
		{
			CrestronUtils.Reboot();
		}

		/// <summary>
		/// Called when the user presses the panel setup button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnPanelSetupButtonPressed(object sender, EventArgs eventArgs)
		{
		}

		/// <summary>
		/// Called when the user presses the load button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnLoadButtonPressed(object sender, EventArgs eventArgs)
		{
			// Load settings in a new thread to avoid disposing this panel in the UI thread.
			m_LoadHandle = CrestronUtils.SafeInvoke(LoadSettings);
		}

		#endregion
	}
}
