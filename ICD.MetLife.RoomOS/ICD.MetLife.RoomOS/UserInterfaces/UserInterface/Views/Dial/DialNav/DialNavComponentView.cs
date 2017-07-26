using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial.DialNav;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial.DialNav
{
	public sealed partial class DialNavComponentView : AbstractComponentView, IDialNavComponentView
	{
		public event EventHandler OnPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public DialNavComponentView(ISigInputOutput panel, IVtProParent parent, ushort index)
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
			m_Label.SetLabelTextAtJoin(m_Label.SerialLabelJoins.First(), text);
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

		/// <summary>
		/// Gets the icon for the given type.
		/// </summary>
		/// <param name="iconType"></param>
		/// <returns></returns>
		public IIcon GetIcon(eDialNavIcon iconType)
		{
			return DialNavIcon.FromDialNavType(iconType);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Button.OnPressed += ButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Button.OnPressed -= ButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ButtonOnPressed(object sender, EventArgs args)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
