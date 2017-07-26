using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Settings;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Settings
{
	public sealed partial class SettingsDevicePropertiesComponentView : AbstractComponentView,
	                                                                    ISettingsDevicePropertiesComponentView
	{
		public event EventHandler OnPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public SettingsDevicePropertiesComponentView(ISigInputOutput panel, IVtProParent parent, ushort index)
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
		/// Sets the property name label text.
		/// </summary>
		/// <param name="name"></param>
		public void SetPropertyName(string name)
		{
			m_PropertyLabel.SetLabelTextAtJoin(m_PropertyLabel.SerialLabelJoins.First(), name);
		}

		/// <summary>
		/// Sets the property value label text.
		/// </summary>
		/// <param name="value"></param>
		public void SetPropertyValue(string value)
		{
			m_PropertyButton.SetLabelTextAtJoin(m_PropertyButton.SerialLabelJoins.First(), value);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_PropertyButton.OnPressed += PropertyButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_PropertyButton.OnPressed -= PropertyButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the property button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PropertyButtonOnPressed(object sender, EventArgs args)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
