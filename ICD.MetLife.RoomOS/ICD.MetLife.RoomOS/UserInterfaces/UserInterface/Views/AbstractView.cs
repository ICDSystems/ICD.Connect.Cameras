﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views
{
	/// <summary>
	/// Base class for all of the views.
	/// </summary>
	public abstract class AbstractView : IView
	{
		public event EventHandler<BoolEventArgs> OnVisibilityChanged;
		public event EventHandler<BoolEventArgs> OnEnabledChanged;

		private IVtProControl m_CachedPage;

		/// <summary>
		/// Returns true if the view is visible.
		/// </summary>
		public bool IsVisible { get { return Page.IsVisible; } }

		/// <summary>
		/// Returns true if the view is enabled.
		/// </summary>
		public bool IsEnabled { get { return Page.IsEnabled; } }

		/// <summary>
		/// Gets the page control.
		/// </summary>
		public IVtProControl Page
		{
			get
			{
				return m_CachedPage ?? (m_CachedPage = GetChildren().FirstOrDefault(c => c is VtProSubpage || c is VtProPage));
			}
		}

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		protected AbstractView(ISigInputOutput panel)
			: this(panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		protected AbstractView(ISigInputOutput panel, IVtProParent parent)
			: this(panel, parent, 0)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractView(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			InstantiateControls(panel, parent, index);
			SubscribeControls();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			OnVisibilityChanged = null;
			OnEnabledChanged = null;

			UnsubscribeControls();

			foreach (IVtProControl control in GetChildren())
				control.Dispose();
		}

		/// <summary>
		/// Sets the visibility of the view.
		/// </summary>
		/// <param name="visible"></param>
		public virtual void Show(bool visible)
		{
			if (visible == IsVisible)
				return;

			try
			{
				Page.Show(visible);
			}
			catch (Exception e)
			{
				string error = string.Format("Unable to show {0} - {1}", GetType().Name, e.Message);
				throw new Exception(error, e);
			}

			OnVisibilityChanged.Raise(this, new BoolEventArgs(visible));
		}

		/// <summary>
		/// Sets the enabled state of the view.
		/// </summary>
		/// <param name="enabled"></param>
		public void Enable(bool enabled)
		{
			if (enabled == IsEnabled)
				return;

			try
			{
				Page.Enable(enabled);
			}
			catch (Exception e)
			{
				string error = string.Format("Unable to enable {0} - {1}", GetType().Name, e.Message);
				throw new Exception(error, e);
			}

			OnEnabledChanged.Raise(this, new BoolEventArgs(enabled));
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerable<IVtProControl> GetChildren();

		#endregion

		#region Private Methods

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected abstract void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index);

		/// <summary>
		/// Generates child views for the given subpage reference list. Sets the number of items in the SRL.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="factory"></param>
		/// <param name="subpageReferenceList"></param>
		/// <param name="viewList"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		protected static IEnumerable<T> GetChildViews<T>(IViewFactory factory, VtProSubpageReferenceList subpageReferenceList,
		                                                 List<T> viewList, ushort count)
			where T : class, IView
		{
			if (factory as MetlifeViewFactory == null)
				throw new ArgumentException("View factory must support SRLs", "factory");

			return (factory as MetlifeViewFactory).GetNewSrlViews(subpageReferenceList, viewList, count);
		}

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected virtual void SubscribeControls()
		{
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected virtual void UnsubscribeControls()
		{
		}

		#endregion
	}
}
