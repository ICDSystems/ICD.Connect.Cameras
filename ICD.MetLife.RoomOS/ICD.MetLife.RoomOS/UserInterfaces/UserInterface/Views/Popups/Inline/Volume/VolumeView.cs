using System;
using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Volume;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Volume
{
	public sealed partial class VolumeView : AbstractView, IVolumeView
	{
		public event EventHandler<BoolEventArgs> OnScrollingChanged;

		private readonly List<IVolumeComponentView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public VolumeView(ISigInputOutput panel)
			: base(panel)
		{
			m_ChildList = new List<IVolumeComponentView>();
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IVolumeComponentView> GetChildVolumeViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_VolumeComponentsList, m_ChildList, count);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_VolumeComponentsList.OnIsMovingChanged += VolumeComponentsListOnIsMovingChanged;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_VolumeComponentsList.OnIsMovingChanged -= VolumeComponentsListOnIsMovingChanged;
		}

		/// <summary>
		/// Called when the volume subpage reference list starts/stops scrolling.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void VolumeComponentsListOnIsMovingChanged(object sender, BoolEventArgs args)
		{
			OnScrollingChanged.Raise(this, new BoolEventArgs(args.Data));
		}

		#endregion
	}
}
