using System;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.ShareMenu;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.ShareMenu
{
	public sealed partial class ShareComponentView : AbstractComponentView, IShareComponentView
	{
		public event EventHandler OnPressed;
		public event EventHandler OnReleased;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ShareComponentView(ISigInputOutput panel, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the source label.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="status"></param>
		public void SetLabel(string name, string status)
		{
			m_Label.SetLabelTextAtJoin(m_Label.SerialLabelJoins[0], name);
			m_Label.SetLabelTextAtJoin(m_Label.SerialLabelJoins[1], status);
		}

		/// <summary>
		/// Sets the icon for the source.
		/// </summary>
		/// <param name="icon"></param>
		/// <param name="state"></param>
		public void SetIcon(IIcon icon, eIconState state)
		{
			string iconSerial = icon.GetIconString(state);
			m_Icon.SetIcon(iconSerial);
		}

		public IIcon GetIcon(eSourceType sourceType)
		{
			return SourceIcon.FromSourceType(sourceType);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribe to the controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Button.OnPressed += ButtonOnPressed;
			m_Button.OnReleased += ButtonOnReleased;
		}

		/// <summary>
		/// Unsubscribe from the controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Button.OnPressed -= ButtonOnPressed;
			m_Button.OnReleased -= ButtonOnReleased;
		}

		/// <summary>
		/// Called when the user presses the component button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ButtonOnPressed(object sender, EventArgs args)
		{
			OnPressed.Raise(this);
		}

		private void ButtonOnReleased(object sender, EventArgs eventArgs)
		{
			OnReleased.Raise(this);
		}

		#endregion
	}
}
