using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Rooms;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Components.Directory;
using ICD.Connect.Conferencing.Cisco.Components.Directory.Tree;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial
{
	public sealed class DirectoryPresenter : AbstractDialPresenter<IDirectoryView>, IDirectoryPresenter
	{
		private readonly DirectoryBrowser m_DirectoryBrowser;

		private readonly List<IDirectoryContactComponentPresenter> m_Contacts;
		private readonly List<IDirectoryFolderComponentPresenter> m_Folders;
		private readonly SafeCriticalSection m_ChildrenSection;

		private readonly Dictionary<IContact, IDirectoryContactComponentPresenter> m_ContactToPresenter;

		private IContact m_Selected;

		/// <summary>
		/// Title for the menu.
		/// </summary>
		protected override string Title { get { return "Contacts"; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public DirectoryPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_Contacts = new List<IDirectoryContactComponentPresenter>();
			m_Folders = new List<IDirectoryFolderComponentPresenter>();
			m_ChildrenSection = new SafeCriticalSection();

			m_ContactToPresenter = new Dictionary<IContact, IDirectoryContactComponentPresenter>();

			CiscoCodec codec = Room.GetDevice<CiscoCodec>();
			if (codec == null)
				return;

			DirectoryComponent component = codec.Components.GetComponent<DirectoryComponent>();
			m_DirectoryBrowser = new DirectoryBrowser(component)
			{
				PhonebookType = Room == null ? default(ePhonebookType) : Room.PhonebookType
			};
			Subscribe(m_DirectoryBrowser);
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			if (m_DirectoryBrowser != null)
			{
				Unsubscribe(m_DirectoryBrowser);
				m_DirectoryBrowser.Dispose();
			}

			DisposeChildren();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IDirectoryView view)
		{
			base.Refresh(view);

			if (m_DirectoryBrowser == null)
				return;

			m_DirectoryBrowser.PopulateCurrentFolder();

			m_ChildrenSection.Enter();

			try
			{
				ClearChildViews();

				// Build the presenters
				IFolder current = m_DirectoryBrowser.GetCurrentFolder();
				IFolder[] folders = current.GetFolders();
				IContact[] contacts = current.GetContacts().Cast<IContact>().ToArray();

				IDirectoryFolderComponentPresenter[] folderPresenters =
					Navigation.GetNewPresenters(m_Folders, folders.Length).ToArray();
				IDirectoryContactComponentPresenter[] contactPresenters =
					Navigation.GetNewPresenters(m_Contacts, contacts.Length).ToArray();

				// Build the views
				ushort length = (ushort)(folders.Length + contacts.Length);
				IFavoritesAndDirectoryComponentView[] views = view.GetChildCallViews(ViewFactory, length).ToArray();
				IFavoritesAndDirectoryComponentView[] folderViews = views.Take(folders.Length).ToArray();
				IFavoritesAndDirectoryComponentView[] contactViews = views.Skip(folders.Length).ToArray();

				// Assign the data
				for (int index = 0; index < folderViews.Length; index++)
				{
					IDirectoryFolderComponentPresenter presenter = folderPresenters[index];
					IFolder folder = folders[index];

					Subscribe(presenter);

					presenter.SetView(folderViews[index]);
					presenter.Folder = folder;
					presenter.ShowView(true);
				}

				for (int index = 0; index < contactViews.Length; index++)
				{
					IDirectoryContactComponentPresenter presenter = contactPresenters[index];
					IContact contact = contacts[index];

					Subscribe(presenter);

					presenter.SetView(contactViews[index]);
					presenter.Contact = contact;
					presenter.Selected = contact == m_Selected;
					presenter.ShowView(true);
				}

				m_ContactToPresenter.Clear();
				m_ContactToPresenter.AddRange(m_Contacts.Where(p => p.Contact != null), p => p.Contact);

				bool isRoot = m_DirectoryBrowser.IsCurrentFolderRoot;
				view.ShowHomeButton(!isRoot);
				view.ShowUpButton(!isRoot);
			}
			finally
			{
				m_ChildrenSection.Leave();
			}

			RefreshDialButton();
		}

		/// <summary>
		/// Sets the given favorite as selected.
		/// </summary>
		/// <param name="contact"></param>
		private void SetSelected(IContact contact)
		{
			if (contact == m_Selected)
				return;

			if (m_Selected != null && m_ContactToPresenter.ContainsKey(m_Selected))
				m_ContactToPresenter[m_Selected].Selected = false;

			m_Selected = contact;

			if (m_Selected != null && m_ContactToPresenter.ContainsKey(m_Selected))
				m_ContactToPresenter[m_Selected].Selected = true;

			RefreshDialButton();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sets the enabled state and label of the dial button.
		/// </summary>
		private void RefreshDialButton()
		{
			eConferenceSourceType type = m_Selected == null || Room == null
				                             ? eConferenceSourceType.Unknown
				                             : Room.ConferenceManager.DialingPlan.GetSourceType(m_Selected);

			string callTypeString = StringUtils.NiceName(type);

			IDirectoryView view = GetView();

			view.SetCallTypeLabel(callTypeString);
			view.EnableDialButton(m_Selected != null);
		}

		/// <summary>
		/// Dispose the child presenters.
		/// </summary>
		private void DisposeChildren()
		{
			foreach (IDirectoryFolderComponentPresenter folder in m_Folders)
			{
				Unsubscribe(folder);
				folder.Dispose();
			}

			foreach (IDirectoryContactComponentPresenter contact in m_Contacts)
			{
				Unsubscribe(contact);
				contact.Dispose();
			}

			m_Folders.Clear();
			m_Contacts.Clear();
		}

		/// <summary>
		/// Sets the child views to null for recycling.
		/// </summary>
		private void ClearChildViews()
		{
			foreach (IDirectoryFolderComponentPresenter folder in m_Folders)
			{
				Unsubscribe(folder);
				folder.SetView(null);
			}

			foreach (IDirectoryContactComponentPresenter contact in m_Contacts)
			{
				Unsubscribe(contact);
				contact.SetView(null);
			}
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Subscribes to the child events.
		/// </summary>
		/// <param name="contact"></param>
		private void Subscribe(IDirectoryContactComponentPresenter contact)
		{
			if (contact == null)
				return;

			contact.OnPressed += ContactOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the child events.
		/// </summary>
		/// <param name="contact"></param>
		private void Unsubscribe(IDirectoryContactComponentPresenter contact)
		{
			if (contact == null)
				return;

			contact.OnPressed -= ContactOnPressed;
		}

		/// <summary>
		/// Called when a child becomes selected or deselected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ContactOnPressed(object sender, EventArgs eventArgs)
		{
			IDirectoryContactComponentPresenter contactPresenter = sender as IDirectoryContactComponentPresenter;
			if (contactPresenter == null)
				return;

			IContact contact = contactPresenter.Contact;

			SetSelected(contact == m_Selected ? null : contact);
		}

		/// <summary>
		/// Subscribes to the child events.
		/// </summary>
		/// <param name="folder"></param>
		private void Subscribe(IDirectoryFolderComponentPresenter folder)
		{
			if (folder == null)
				return;

			folder.OnPressed += FolderOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the child events.
		/// </summary>
		/// <param name="folder"></param>
		private void Unsubscribe(IDirectoryFolderComponentPresenter folder)
		{
			if (folder == null)
				return;

			folder.OnPressed -= FolderOnPressed;
		}

		/// <summary>
		/// Called when a child becomes selected or deselected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void FolderOnPressed(object sender, EventArgs eventArgs)
		{
			IDirectoryFolderComponentPresenter folderPresenter = sender as IDirectoryFolderComponentPresenter;
			if (folderPresenter == null)
				return;

			if (m_DirectoryBrowser != null)
				m_DirectoryBrowser.EnterFolder(folderPresenter.Folder);
		}

		#endregion

		#region Directory Browser Callbacks

		/// <summary>
		/// Subscribe to the browser events.
		/// </summary>
		/// <param name="browser"></param>
		private void Subscribe(DirectoryBrowser browser)
		{
			if (browser == null)
				return;

			browser.OnPathChanged += BrowserOnPathChanged;
			browser.OnPathContentsChanged += BrowserOnPathContentsChanged;
		}

		/// <summary>
		/// Unsubscribe from the browser events.
		/// </summary>
		/// <param name="browser"></param>
		private void Unsubscribe(DirectoryBrowser browser)
		{
			if (browser == null)
				return;

			browser.OnPathChanged -= BrowserOnPathChanged;
			browser.OnPathContentsChanged -= BrowserOnPathContentsChanged;
		}

		/// <summary>
		/// Called when the curent folder contents change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void BrowserOnPathContentsChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the browser path changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="folderEventArgs"></param>
		private void BrowserOnPathChanged(object sender, FolderEventArgs folderEventArgs)
		{
			m_Selected = null;

			if (m_DirectoryBrowser != null)
				m_DirectoryBrowser.PopulateCurrentFolder();

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IDirectoryView view)
		{
			base.Subscribe(view);

			view.OnDialButtonPressed += ViewOnDialButtonPressed;
			view.OnHomeButtonPressed += ViewOnHomeButtonPressed;
			view.OnUpButtonPressed += ViewOnUpButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IDirectoryView view)
		{
			base.Unsubscribe(view);

			view.OnDialButtonPressed -= ViewOnDialButtonPressed;
			view.OnHomeButtonPressed -= ViewOnHomeButtonPressed;
			view.OnUpButtonPressed -= ViewOnUpButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnUpButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_DirectoryBrowser != null)
				m_DirectoryBrowser.GoUp();
		}

		/// <summary>
		/// Called when the user presses the home button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnHomeButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_DirectoryBrowser != null)
				m_DirectoryBrowser.GoToRoot();
		}

		/// <summary>
		/// Called when the user presses the dial button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDialButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_Selected != null)
				Dial(m_Selected);
			m_Selected = null;
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			m_Selected = null;

			base.ViewOnVisibilityChanged(sender, args);
		}

		#endregion
	}
}
