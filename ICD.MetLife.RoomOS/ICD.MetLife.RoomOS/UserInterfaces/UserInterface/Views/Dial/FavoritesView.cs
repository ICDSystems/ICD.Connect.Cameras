using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial
{
	public sealed partial class FavoritesView : AbstractView, IFavoritesView
	{
		public event EventHandler OnDialButtonPressed;

		private readonly List<IFavoritesAndDirectoryComponentView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public FavoritesView(ISigInputOutput panel)
			: base(panel)
		{
			m_ChildList = new List<IFavoritesAndDirectoryComponentView>();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDialButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the call type text on the dial button (i.e. Audio, Video)
		/// </summary>
		/// <param name="type"></param>
		public void SetCallTypeLabel(string type)
		{
			m_DialButton.SetLabelTextAtJoin(m_DialButton.SerialLabelJoins.First(), type);
		}

		/// <summary>
		/// Sets the enabled state of the dial button.
		/// </summary>
		/// <param name="enabled"></param>
		public void EnableDialButton(bool enabled)
		{
			m_DialButton.Enable(enabled);
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IFavoritesAndDirectoryComponentView> GetChildCallViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_FavoritesList, m_ChildList, count);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DialButton.OnPressed += DialButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DialButton.OnPressed -= DialButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the dial button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DialButtonOnPressed(object sender, EventArgs args)
		{
			OnDialButtonPressed.Raise(this);
		}

		#endregion
	}
}
