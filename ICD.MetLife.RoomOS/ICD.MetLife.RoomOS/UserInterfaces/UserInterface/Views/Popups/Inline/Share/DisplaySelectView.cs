using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Panels;
using ICD.Common.EventArguments;
using ICD.Connect.UI.Controls;
using ICD.Common.Utils.Extensions;
using ICD.MetLife.RoomOS.UserInterfaces.UserInterface.IViews.Popups.Inline.Share;

namespace ICD.MetLife.RoomOS.UserInterfaces.UserInterface.Views.Popups.Inline.Share
{
	public sealed partial class DisplaySelectView : AbstractView, IDisplaySelectView
	{
		public event EventHandler OnListenToSourceButtonPressed;
		public event EventHandler OnShowVideoCallButtonPressed;
		public event EventHandler<UShortEventArgs> OnDestinationButtonPressed;
		public event EventHandler<BoolEventArgs> OnDestinationsMovingChanged;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public DisplaySelectView(ISigInputOutput panel)
			: base(panel)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public DisplaySelectView(ISigInputOutput panel, IVtProParent parent)
			: base(panel, parent)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public DisplaySelectView(ISigInputOutput panel, IVtProParent parent, ushort index)
			: base(panel, parent, index)
		{
		}

		#endregion

		#region Methods

		public void SetSourceName(string name)
		{
			m_ShareSourceLabel.SetLabelTextAtJoin(m_ShareSourceLabel.SerialLabelJoins.First(), name);
		}

		public void SetListenToSourceButtonEnabled(bool enable)
		{
			m_ListenToSourceButton.Enable(enable);
		}

		public void SetShowVideoCallButtonEnabled(bool enable)
		{
			m_ShowVideoCallButton.Enable(enable);
		}

		public void SetShowVideoCallButtonVisible(bool visible)
		{
			m_ShowVideoCallButton.Show(visible);
		}

		public void SetDestinationLabels(IEnumerable<string> labels)
		{
			string[] items = labels as string[] ?? labels.ToArray();

			m_DestinationList.SetItemLabels(items);

			// Enable the buttons
			int length = Math.Min(items.Length, m_DestinationList.MaxSize);
			for (ushort index = 0; index < length; index++)
				m_DestinationList.SetItemEnabled(index, true);
		}

		public void SetDestinationSelected(ushort index, bool selected)
		{
			m_DestinationList.SetItemSelected(index, selected);
		}

		public void SetDestinationVisible(ushort index, bool visible)
		{
			m_DestinationList.SetItemVisible(index, visible);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ListenToSourceButton.OnPressed += ListenToSourceButtonOnPressed;
			m_ShowVideoCallButton.OnPressed += ShowVideoCallButtonOnPressed;
			m_DestinationList.OnButtonClicked += DestinationListOnButtonClicked;
			m_DestinationList.OnIsMovingChanged += DestinationListOnIsMovingChanged;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ListenToSourceButton.OnPressed -= ListenToSourceButtonOnPressed;
			m_ShowVideoCallButton.OnPressed -= ShowVideoCallButtonOnPressed;
			m_DestinationList.OnButtonClicked -= DestinationListOnButtonClicked;
			m_DestinationList.OnIsMovingChanged -= DestinationListOnIsMovingChanged;
		}

		private void DestinationListOnIsMovingChanged(object sender, BoolEventArgs args)
		{
			OnDestinationsMovingChanged.Raise(this, new BoolEventArgs(args.Data));
		}

		private void DestinationListOnButtonClicked(object sender, UShortEventArgs args)
		{
			OnDestinationButtonPressed.Raise(this, new UShortEventArgs(args.Data));
		}

		private void ShowVideoCallButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnShowVideoCallButtonPressed.Raise(this);
		}

		private void ListenToSourceButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnListenToSourceButtonPressed.Raise(this);
		}

		#endregion
	}
}
