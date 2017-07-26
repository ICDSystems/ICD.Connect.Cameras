using System;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.TvTuner;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.TvTuner
{
	public sealed partial class ChannelPresetView : AbstractComponentView, IChannelPresetView
	{
		public event EventHandler OnPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ChannelPresetView(ISigInputOutput panel, IVtProParent parent, ushort index)
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
		/// Sets the icon image by path.
		/// </summary>
		/// <param name="path"></param>
		public void SetImage(string path)
		{
			m_Icon.SetIconPath(path);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Icon.OnPressed += IconOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Icon.OnPressed -= IconOnPressed;
		}

		/// <summary>
		/// Called when the user presses the icon.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void IconOnPressed(object sender, EventArgs args)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
