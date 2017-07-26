using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;
using ICD.Connect.UI.Controls.Buttons;

namespace ICD.Connect.UI.Controls.Guages
{
	public abstract class AbstractVtProGuage : AbstractVtProButton
	{
		public event EventHandler<UShortEventArgs> OnTouched;

		private readonly SafeCriticalSection m_SetValueSection;

		private ushort m_AnalogFeedbackJoin;
		private ushort m_SubscribedAnalogFeedbackJoin;

		private ushort m_ValueCache;

		/// <summary>
		/// Gets the analog touch join.
		/// </summary>
		[PublicAPI]
		public ushort AnalogFeedbackJoin
		{
			get { return m_AnalogFeedbackJoin; }
			set
			{
				if (value == m_AnalogFeedbackJoin)
					return;

				UnsubscribeAnalogFeedback();
				m_AnalogFeedbackJoin = value;
				SubscribeAnalogFeedback();
			}
		}

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		protected AbstractVtProGuage(ISigInputOutput panel)
			: this(panel, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		protected AbstractVtProGuage(ISigInputOutput panel, IVtProParent parent)
			: base(panel, parent)
		{
			m_SetValueSection = new SafeCriticalSection();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnTouched = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the value shown by the guage.
		/// </summary>
		/// <param name="value"></param>
		[PublicAPI]
		public void SetValue(ushort value)
		{
			m_SetValueSection.Enter();

			try
			{
				if (AnalogFeedbackJoin == 0)
					throw new InvalidOperationException();

				if (value == m_ValueCache)
					return;

				ushort join = Parent == null ? AnalogFeedbackJoin : Parent.GetAnalogJoin(AnalogFeedbackJoin, this);

				m_ValueCache = value;
				Panel.SendInputAnalog(join, m_ValueCache);
			}
			finally
			{
				m_SetValueSection.Leave();
			}
		}

		/// <summary>
		/// Sets the value in the range 0.0f to 1.0f
		/// </summary>
		/// <param name="percentage"></param>
		public void SetValuePercentage(float percentage)
		{
			ushort value = (ushort)(percentage * ushort.MaxValue);
			SetValue(value);
		}

		/// <summary>
		/// Simulates a touch on the guage.
		/// </summary>
		/// <param name="value"></param>
		[PublicAPI]
		public void Touch(ushort value)
		{
			if (IsVisibleRecursive)
				OnTouched.Raise(this, new UShortEventArgs(value));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribe to the panel events.
		/// </summary>
		protected override void SubscribePanelFeedback()
		{
			base.SubscribePanelFeedback();

			SubscribeAnalogFeedback();
		}

		/// <summary>
		/// Unsubscribe from the panel events.
		/// </summary>
		protected override void UnsubscribePanelFeedback()
		{
			base.UnsubscribePanelFeedback();

			UnsubscribeAnalogFeedback();
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeAnalogFeedback()
		{
			if (m_SubscribedAnalogFeedbackJoin != 0)
				Panel.UnregisterOutputSigChangeCallback(m_SubscribedAnalogFeedbackJoin, eSigType.Analog, AnalogFeedback);
			m_SubscribedAnalogFeedbackJoin = 0;
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join and subscribes to the current join.
		/// </summary>
		private void SubscribeAnalogFeedback()
		{
			UnsubscribeAnalogFeedback();

			m_SubscribedAnalogFeedbackJoin = Parent == null ? AnalogFeedbackJoin : Parent.GetAnalogJoin(AnalogFeedbackJoin, this);
			if (m_SubscribedAnalogFeedbackJoin != 0)
				Panel.RegisterOutputSigChangeCallback(m_SubscribedAnalogFeedbackJoin, eSigType.Analog, AnalogFeedback);
		}

		/// <summary>
		/// Called when the guage is touched.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void AnalogFeedback(object parent, SigAdapterEventArgs args)
		{
			if (AnalogFeedbackJoin == 0)
				return;

			Touch(args.Data.GetUShortValue());
		}

		#endregion
	}
}
