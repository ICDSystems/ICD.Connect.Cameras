using System;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Dial
{
	public sealed partial class FavoritesAndDirectoryComponentView : AbstractComponentView,
	                                                                 IFavoritesAndDirectoryComponentView
	{
		private const ushort MODE_AUDIO = 0;
		private const ushort MODE_VIDEO = 1;
		private const ushort MODE_FOLDER = 2;
		private const ushort MODE_USER = 3;

		public event EventHandler OnPressed;
		public event EventHandler OnFavoriteButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public FavoritesAndDirectoryComponentView(ISigInputOutput panel, IVtProParent parent, ushort index)
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
			OnFavoriteButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the icon for the contact.
		/// </summary>
		/// <param name="icon"></param>
		public void SetIcon(eRecentCallIconMode icon)
		{
			switch (icon)
			{
				case eRecentCallIconMode.Audio:
					m_IconButton.SetMode(MODE_AUDIO);
					break;
				case eRecentCallIconMode.Video:
					m_IconButton.SetMode(MODE_VIDEO);
					break;
				case eRecentCallIconMode.User:
					m_IconButton.SetMode(MODE_USER);
					break;
				case eRecentCallIconMode.Folder:
					m_IconButton.SetMode(MODE_FOLDER);
					break;
				default:
					throw new ArgumentOutOfRangeException("icon");
			}
		}

		/// <summary>
		/// Sets the name of the contact.
		/// </summary>
		/// <param name="name"></param>
		public void SetName(string name)
		{
			m_Text.SetLabelTextAtJoin(m_Text.SerialLabelJoins.First(), name);
		}

		/// <summary>
		/// Sets the favorite state of the contact.
		/// </summary>
		/// <param name="favorite"></param>
		public void SetFavorite(bool favorite)
		{
			m_FavoriteButton.SetSelected(favorite);
		}

		/// <summary>
		/// Sets the visibility of the favorite button.
		/// </summary>
		/// <param name="show"></param>
		public void ShowFavoriteButton(bool show)
		{
			m_FavoriteButton.Show(show);
		}

		/// <summary>
		/// Sets the selected state of the component.
		/// </summary>
		/// <param name="selected"></param>
		public void SetSelected(bool selected)
		{
			m_BackgroundButton.SetSelected(selected);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_BackgroundButton.OnPressed += BackgroundButtonOnPressed;
			m_FavoriteButton.OnPressed += FavoriteButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_BackgroundButton.OnPressed += BackgroundButtonOnPressed;
			m_FavoriteButton.OnPressed += FavoriteButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the favorite button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void FavoriteButtonOnPressed(object sender, EventArgs args)
		{
			OnFavoriteButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the background button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void BackgroundButtonOnPressed(object sender, EventArgs args)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
