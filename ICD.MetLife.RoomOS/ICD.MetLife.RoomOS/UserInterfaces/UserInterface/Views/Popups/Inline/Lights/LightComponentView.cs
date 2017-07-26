using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Lights;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Lights
{
	public sealed partial class LightComponentView : AbstractComponentView, ILightComponentView
	{
		public event EventHandler OnUpButtonPressed;
		public event EventHandler OnDownButtonPressed;
		public event EventHandler OnButtonReleased;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public LightComponentView(ISigInputOutput panel, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnUpButtonPressed = null;
			OnDownButtonPressed = null;
			OnButtonReleased = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the value on the bar in the range 0.0f - 1.0f
		/// </summary>
		/// <param name="level"></param>
		public void SetPercentage(float level)
		{
			m_Guage.SetValuePercentage(level);
		}

		/// <summary>
		/// Sets the title text.
		/// </summary>
		/// <param name="title"></param>
		public void SetTitle(string title)
		{
			m_Title.SetLabelTextAtJoin(m_Title.SerialLabelJoins.First(), title);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_UpButton.OnPressed += UpButtonOnPressed;
			m_UpButton.OnReleased += UpButtonOnReleased;
			m_DownButton.OnPressed += DownButtonOnPressed;
			m_DownButton.OnReleased += DownButtonOnReleased;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_UpButton.OnPressed -= UpButtonOnPressed;
			m_UpButton.OnReleased -= UpButtonOnReleased;
			m_DownButton.OnPressed -= DownButtonOnPressed;
			m_DownButton.OnReleased -= DownButtonOnReleased;
		}

		private void DownButtonOnReleased(object sender, EventArgs eventArgs)
		{
			OnButtonReleased.Raise(this);
		}

		private void UpButtonOnReleased(object sender, EventArgs eventArgs)
		{
			OnButtonReleased.Raise(this);
		}

		private void DownButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDownButtonPressed.Raise(this);
		}

		private void UpButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnUpButtonPressed.Raise(this);
		}

		#endregion
	}
}
