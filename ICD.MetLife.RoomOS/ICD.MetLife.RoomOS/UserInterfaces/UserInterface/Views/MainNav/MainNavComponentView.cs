using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.Endpoints.Sources;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.MainNav;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.MainNav
{
	public sealed partial class MainNavComponentView : AbstractComponentView, IMainNavComponentView
	{
		public event EventHandler OnPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public MainNavComponentView(ISigInputOutput panel, IVtProParent parent, ushort index)
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
		/// Sets the label text for the component.
		/// </summary>
		/// <param name="text"></param>
		public void SetLabelText(string text)
		{
			m_LabelText.SetLabelTextAtJoin(m_LabelText.SerialLabelJoins.First(), text);
		}

		/// <summary>
		/// Sets the icon for the component.
		/// </summary>
		/// <param name="icon"></param>
		/// <param name="state"></param>
		public void SetIcon(IIcon icon, eIconState state)
		{
			string iconSerial = icon.GetIconString(state);
			m_Icon.SetIcon(iconSerial);
		}

		public IIcon GetIcon(eMainNavIcon iconType)
		{
			return MainNavIcon.FromMainNavType(iconType);
		}

		public IIcon GetIcon(eSourceType sourceType)
		{
			return SourceIcon.FromSourceType(sourceType);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_MainButton.OnPressed += MainButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_MainButton.OnPressed -= MainButtonOnPressed;
		}

		/// <summary>
		/// Called when the main button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void MainButtonOnPressed(object sender, EventArgs args)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
