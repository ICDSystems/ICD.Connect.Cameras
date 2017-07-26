using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.UI.Controls.TextControls
{
	public sealed class VtProTextEntry : AbstractVtProLabel
	{
		public event EventHandler<StringEventArgs> OnTextModified;

		private readonly SafeCriticalSection m_SetModeSection;

		private ushort m_SerialOutputJoin;
		private ushort m_SubscribedSerialOutputJoin;

		private ushort m_CachedMode;

		#region Properties

		[PublicAPI]
		public ushort EnterKeyPressJoin { get; set; }

		[PublicAPI]
		public ushort EscKeyPressJoin { get; set; }

		[PublicAPI]
		public ushort AnalogModeJoin { get; set; }

		[PublicAPI]
		public ushort SetFocusJoinOn { get; set; }

		[PublicAPI]
		public ushort SetFocusJoinOff { get; set; }

		[PublicAPI]
		public ushort HasFocusJoin { get; set; }

		[PublicAPI]
		public ushort SerialOutputJoin
		{
			get { return m_SerialOutputJoin; }
			set
			{
				if (value == m_SerialOutputJoin)
					return;

				UnsubscribeSerialFeedback();
				m_SerialOutputJoin = value;
				SubscribeSerialFeedback();
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		public VtProTextEntry(ISigInputOutput panel)
			: this(panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		public VtProTextEntry(ISigInputOutput panel, IVtProParent parent)
			: base(panel, parent)
		{
			m_SetModeSection = new SafeCriticalSection();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnTextModified = null;

			base.Dispose();
		}

		/// <summary>
		/// Simulates text input and raises the OnTextModified event.
		/// </summary>
		[PublicAPI]
		public void SetTextEntry(string text)
		{
			if (IsVisibleRecursive)
				OnTextModified.Raise(this, new StringEventArgs(text));
		}

		/// <summary>
		/// Sets the button mode. Throws InvalidOperationException if there is no mode join.
		/// </summary>
		/// <param name="mode"></param>
		[PublicAPI]
		public void SetMode(ushort mode)
		{
			m_SetModeSection.Enter();

			try
			{
				if (AnalogModeJoin == 0)
					throw new InvalidOperationException();

				if (mode == m_CachedMode)
					return;

				ushort join = Parent == null ? AnalogModeJoin : Parent.GetAnalogJoin(AnalogModeJoin, this);

				m_CachedMode = mode;
				Panel.SendInputAnalog(join, m_CachedMode);
			}
			finally
			{
				m_SetModeSection.Leave();
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

			SubscribeSerialFeedback();
		}

		/// <summary>
		/// Unsubscribe from the panel events.
		/// </summary>
		protected override void UnsubscribePanelFeedback()
		{
			base.UnsubscribePanelFeedback();

			UnsubscribeSerialFeedback();
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeSerialFeedback()
		{
			if (m_SubscribedSerialOutputJoin != 0)
				Panel.UnregisterOutputSigChangeCallback(m_SubscribedSerialOutputJoin, eSigType.Serial, TextInputChange);
			m_SubscribedSerialOutputJoin = 0;
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join and subscribes to the current join.
		/// </summary>
		private void SubscribeSerialFeedback()
		{
			UnsubscribeSerialFeedback();

			m_SubscribedSerialOutputJoin = Parent == null ? SerialOutputJoin : Parent.GetSerialJoin(SerialOutputJoin, this);

			if (m_SubscribedSerialOutputJoin == 0)
				return;

			Panel.RegisterOutputSigChangeCallback(m_SubscribedSerialOutputJoin, eSigType.Serial, TextInputChange);
		}

		/// <summary>
		/// Called when text is entered.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void TextInputChange(object parent, SigAdapterEventArgs args)
		{
			if (SerialOutputJoin == 0)
				return;

			SetTextEntry(args.Data.GetStringValue());
		}

		#endregion
	}
}
