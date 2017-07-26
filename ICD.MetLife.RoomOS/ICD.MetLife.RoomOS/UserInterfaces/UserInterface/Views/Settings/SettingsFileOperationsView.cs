using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsFileOperationsView : AbstractView, ISettingsFileOperationsView
	{
		public event EventHandler OnPanelSetupButtonPressed;
		public event EventHandler OnProgramResetButtonPressed;
		public event EventHandler OnProcessorResetButtonPressed;
		public event EventHandler OnSaveAndRunButtonPressed;
		public event EventHandler OnUndoButtonPressed;
		public event EventHandler OnLoadButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public SettingsFileOperationsView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPanelSetupButtonPressed = null;
			OnProgramResetButtonPressed = null;
			OnProcessorResetButtonPressed = null;
			OnSaveAndRunButtonPressed = null;
			OnUndoButtonPressed = null;
			OnLoadButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the program information text.
		/// </summary>
		/// <param name="text"></param>
		public void SetProgramInfoText(string text)
		{
			m_ProgramInfoText.SetLabelTextAtJoin(m_ProgramInfoText.SerialLabelJoins.First(), text);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ApplyButton.OnPressed += ApplyButtonOnPressed;
			m_LoadButton.OnPressed += LoadButtonOnPressed;
			m_PanelSetupButton.OnPressed += PanelSetupButtonOnPressed;
			m_ProcessorResetButton.OnPressed += ProcessorResetButtonOnPressed;
			m_ProgramResetButton.OnPressed += ProgramResetButtonOnPressed;
			m_RevertButton.OnPressed += RevertButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ApplyButton.OnPressed -= ApplyButtonOnPressed;
			m_LoadButton.OnPressed -= LoadButtonOnPressed;
			m_PanelSetupButton.OnPressed -= PanelSetupButtonOnPressed;
			m_ProcessorResetButton.OnPressed -= ProcessorResetButtonOnPressed;
			m_ProgramResetButton.OnPressed -= ProgramResetButtonOnPressed;
			m_RevertButton.OnPressed -= RevertButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the revert button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void RevertButtonOnPressed(object sender, EventArgs args)
		{
			OnUndoButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the program reset button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ProgramResetButtonOnPressed(object sender, EventArgs args)
		{
			OnProgramResetButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the processor button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ProcessorResetButtonOnPressed(object sender, EventArgs args)
		{
			OnProcessorResetButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the panel setup button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PanelSetupButtonOnPressed(object sender, EventArgs args)
		{
			OnPanelSetupButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the load button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void LoadButtonOnPressed(object sender, EventArgs args)
		{
			OnLoadButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the apply button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ApplyButtonOnPressed(object sender, EventArgs args)
		{
			OnSaveAndRunButtonPressed.Raise(this);
		}

		#endregion
	}
}
