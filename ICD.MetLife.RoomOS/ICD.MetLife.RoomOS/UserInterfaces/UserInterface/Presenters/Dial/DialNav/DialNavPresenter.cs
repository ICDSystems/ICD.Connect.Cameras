using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings.Core;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial.DialNav;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IPresenters.Dial.DialNav.Components;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Dial.DialNav;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Presenters.Dial.DialNav
{
	public sealed class DialNavPresenter : AbstractPresenter<IDialNavView>, IDialNavPresenter
	{
		/// <summary>
		/// Ordered list of the components contained in the nav bar.
		/// </summary>
		private static readonly Type[] s_ChildrenTypes =
		{
			typeof(IDialNavDialpadComponentPresenter),
			typeof(IDialNavKeyboardComponentPresenter),
			typeof(IDialNavRecentCallsComponentPresenter),
			//typeof(IDialNavFavoritesComponentPresenter),
			typeof(IDialNavContactsComponentPresenter)
		};

		private readonly List<IDialNavComponentPresenter> m_Children;
		private readonly SafeCriticalSection m_ChildrenSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="core"></param>
		public DialNavPresenter(int room, INavigationController nav, IViewFactory views, ICore core)
			: base(room, nav, views, core)
		{
			m_Children = new List<IDialNavComponentPresenter>();
			m_ChildrenSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			DisposeChildren();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IDialNavView view)
		{
			base.Refresh(view);

			m_ChildrenSection.Enter();

			try
			{
				if (m_Children.Count == 0)
					BuildChildComponents();
			}
			finally
			{
				m_ChildrenSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Builds the nav bar components.
		/// </summary>
		private void BuildChildComponents()
		{
			// Build the presenters and views
			IDialNavComponentPresenter[] presenters = DialNavComponentFactory();
			IDialNavComponentView[] views = GetView().GetChildComponentViews(ViewFactory, (ushort)presenters.Length).ToArray();

			// Bind the views
			for (int index = 0; index < views.Length; index++)
			{
				IDialNavComponentPresenter presenter = presenters[index];
				presenter.SetView(views[index]);
				presenter.ShowView(true);
				presenter.SetViewEnabled(true);
			}

			m_Children.AddRange(presenters);
		}

		/// <summary>
		/// Builds the main nav component presenters.
		/// </summary>
		/// <returns></returns>
		private IDialNavComponentPresenter[] DialNavComponentFactory()
		{
			IDialNavComponentPresenter[] output = new IDialNavComponentPresenter[s_ChildrenTypes.Length];

			for (int index = 0; index < s_ChildrenTypes.Length; index++)
			{
				Type type = s_ChildrenTypes[index];
				output[index] = Navigation.GetNewPresenter(type) as IDialNavComponentPresenter;
			}

			return output;
		}

		/// <summary>
		/// Disposes the child presenters.
		/// </summary>
		private void DisposeChildren()
		{
			foreach (IDialNavComponentPresenter presenter in m_Children)
				presenter.Dispose();
			m_Children.Clear();
		}

		#endregion

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			// HomeButton is visible while the nav is visible.
			Navigation.LazyLoadPresenter<IDialNavHomeButtonPresenter>().ShowView(args.Data);

			if (args.Data)
				return;

			// Hide all of the component menus.
			foreach (IDialNavComponentPresenter component in m_Children)
				component.ShowMenu(false);
		}
	}
}
