using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsBaseView : AbstractView, ISettingsBaseView
	{
		public event EventHandler<UShortEventArgs> OnItemPressed;
		public event EventHandler OnExitButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public SettingsBaseView(ISigInputOutput panel)
			: base(panel)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnItemPressed = null;
			OnExitButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the button labels.
		/// </summary>
		/// <param name="labels"></param>
		public void SetButtonLabels(IEnumerable<string> labels)
		{
			string[] labelsArray = labels.Take(m_ButtonList.MaxSize).ToArray();

			m_ButtonList.SetNumberOfItems((ushort)labelsArray.Length);
			for (ushort index = 0; index < labelsArray.Length; index++)
			{
				m_ButtonList.SetItemVisible(index, true);
				m_ButtonList.SetItemLabel(index, labelsArray[index]);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ExitButton.OnPressed += ExitButtonOnPressed;
			m_ButtonList.OnButtonClicked += ButtonListOnButtonClicked;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ExitButton.OnPressed -= ExitButtonOnPressed;
			m_ButtonList.OnButtonClicked -= ButtonListOnButtonClicked;
		}

		/// <summary>
		/// Called when the user presses a button in the list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ButtonListOnButtonClicked(object sender, UShortEventArgs args)
		{
			OnItemPressed.Raise(this, new UShortEventArgs(args.Data));
		}

		/// <summary>
		/// Called when the user presses the exit button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ExitButtonOnPressed(object sender, EventArgs args)
		{
			OnExitButtonPressed.Raise(this);
		}

		#endregion
	}
}
