using ICD.Connect.Settings.Core;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.Connect.Conferencing.Cisco.Components.Directory.Tree;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public sealed class DirectoryFolderComponentPresenter : AbstractDirectoryComponentPresenter,
	                                                        IDirectoryFolderComponentPresenter
	{
		private IFolder m_Folder;

		/// <summary>
		/// Gets/sets the folder.
		/// </summary>
		public IFolder Folder
		{
			get { return m_Folder; }
			set
			{
				if (value == m_Folder)
					return;

				m_Folder = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public DirectoryFolderComponentPresenter(int room, INavigationController nav, IViewFactory views,
		                                         ICore core)
			: base(room, nav, views, core)
		{
		}

		/// <summary>
		/// Returns true if the component is favorited.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsFavorite()
		{
			return false;
		}

		/// <summary>
		/// Returns the icon for the component.
		/// </summary>
		/// <returns></returns>
		protected override eRecentCallIconMode GetIcon()
		{
			return eRecentCallIconMode.Folder;
		}

		/// <summary>
		/// Returns true if the favorite button should be visible.
		/// </summary>
		/// <returns></returns>
		protected override bool GetFavoriteButtonVisible()
		{
			return false;
		}

		/// <summary>
		/// Returns the name for the component.
		/// </summary>
		/// <returns></returns>
		protected override string GetName()
		{
			return m_Folder == null ? string.Empty : m_Folder.Name;
		}
	}
}
