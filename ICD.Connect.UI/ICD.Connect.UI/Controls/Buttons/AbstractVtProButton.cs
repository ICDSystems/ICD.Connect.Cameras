using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Connect.UI.Controls.Buttons
{
	public abstract class AbstractVtProButton : AbstractVtProLabel
	{
		public event EventHandler OnPressed;
		public event EventHandler OnReleased;

		private readonly SafeCriticalSection m_SelectSection;

		private ushort m_DigitalPressJoin;
		private ushort m_SubscribedDigitalPressJoin;

		private bool m_SelectedCache;

		#region Properties

		/// <summary>
		/// Gets the digital press join.
		/// </summary>
		[PublicAPI]
		public ushort DigitalPressJoin
		{
			get { return m_DigitalPressJoin; }
			set
			{
				if (value == m_DigitalPressJoin)
					return;

				UnsubscribeDigitalFeedback();
				m_DigitalPressJoin = value;
				SubscribeDigitalFeedback();
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		protected AbstractVtProButton(ISigInputOutput panel)
			: this(panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		protected AbstractVtProButton(ISigInputOutput panel, IVtProParent parent)
			: base(panel, parent)
		{
			m_SelectSection = new SafeCriticalSection();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;
			OnReleased = null;

			base.Dispose();
		}

		/// <summary>
		/// Simulates a button press.
		/// </summary>
		[PublicAPI]
		public void Press()
		{
			if (IsVisibleRecursive)
				OnPressed.Raise(this);
		}

		/// <summary>
		/// Simulates a button release.
		/// </summary>
		[PublicAPI]
		public void Release()
		{
			if (IsVisibleRecursive)
				OnReleased.Raise(this);
		}

		/// <summary>
		/// Sets the selected state of the button.
		/// </summary>
		/// <param name="state"></param>
		public void SetSelected(bool state)
		{
			m_SelectSection.Enter();

			try
			{
				if (DigitalPressJoin == 0)
					throw new InvalidOperationException();

				if (state == m_SelectedCache)
					return;

				ushort join = Parent == null ? DigitalPressJoin : Parent.GetDigitalJoin(DigitalPressJoin, this);

				m_SelectedCache = state;
				Panel.SendInputDigital(join, m_SelectedCache);
			}
			finally
			{
				m_SelectSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribe to the panel events.
		/// </summary>
		protected override void SubscribePanelFeedback()
		{
			base.SubscribePanelFeedback();

			SubscribeDigitalFeedback();
		}

		/// <summary>
		/// Unsubscribe from the panel events.
		/// </summary>
		protected override void UnsubscribePanelFeedback()
		{
			base.UnsubscribePanelFeedback();

			UnsubscribeDigitalFeedback();
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeDigitalFeedback()
		{
			if (m_SubscribedDigitalPressJoin != 0)
				Panel.UnregisterOutputSigChangeCallback(m_SubscribedDigitalPressJoin, eSigType.Digital, ButtonPressChange);
			m_SubscribedDigitalPressJoin = 0;
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join and subscribes to the current join.
		/// </summary>
		private void SubscribeDigitalFeedback()
		{
			UnsubscribeDigitalFeedback();

			m_SubscribedDigitalPressJoin = Parent == null ? DigitalPressJoin : Parent.GetDigitalJoin(DigitalPressJoin, this);
			if (m_SubscribedDigitalPressJoin != 0)
				Panel.RegisterOutputSigChangeCallback(m_SubscribedDigitalPressJoin, eSigType.Digital, ButtonPressChange);
		}

		/// <summary>
		/// Called when the press join state changes.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void ButtonPressChange(object parent, SigAdapterEventArgs args)
		{
			if (DigitalPressJoin == 0)
				return;

			if (args.Data.GetBoolValue())
				Press();
			else
				Release();
		}

		#endregion
	}
}
