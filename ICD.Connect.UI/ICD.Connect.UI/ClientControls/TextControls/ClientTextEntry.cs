using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.PanelClients;

namespace ICD.Connect.UI.ClientControls.TextControls
{
	public sealed class ClientTextEntry : AbstractClientLabel
	{
		[PublicAPI]
		public event EventHandler<UShortEventArgs> OnSetMode;

		private ushort m_AnalogModeJoin;
		private ushort m_SubscribedAnalogModeJoin;

		[PublicAPI]
		public ushort AnalogModeJoin
		{
			get { return m_AnalogModeJoin; }
			set
			{
				if (value == m_AnalogModeJoin)
					return;

				UnsubscribeAnalogFeedback();
				m_AnalogModeJoin = value;
				SubscribeAnalogFeedback();
			}
		}

		[PublicAPI]
		public ushort SerialOutputJoin { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public ClientTextEntry(ISigInputOutputClient client)
			: base(client)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		public ClientTextEntry(ISigInputOutputClient client, IJoinOffsets parent)
			: base(client, parent)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ClientTextEntry(ISigInputOutputClient client, IJoinOffsets parent, ushort index)
			: base(client, parent, index)
		{
		}

		public void SetText(string text)
		{
			Client.SendOutputSerial(SerialOutputJoin, text);
		}

		#region Private Methods

		/// <summary>
		/// Subscribe to the panel events.
		/// </summary>
		protected override void SubscribeClientFeedback()
		{
			base.SubscribeClientFeedback();

			SubscribeAnalogFeedback();
		}

		/// <summary>
		/// Unsubscribe from the panel events.
		/// </summary>
		protected override void UnsubscribeClientFeedback()
		{
			base.UnsubscribeClientFeedback();

			UnsubscribeAnalogFeedback();
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeAnalogFeedback()
		{
			if (m_SubscribedAnalogModeJoin != 0)
				Client.UnregisterInputSigChangeCallback(m_SubscribedAnalogModeJoin, eSigType.Analog, ModeChange);
			m_SubscribedAnalogModeJoin = 0;
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join and subscribes to the current join.
		/// </summary>
		private void SubscribeAnalogFeedback()
		{
			UnsubscribeAnalogFeedback();

			m_SubscribedAnalogModeJoin = Parent == null ? AnalogModeJoin : Parent.GetAnalogJoin(AnalogModeJoin, this);
			if (m_SubscribedAnalogModeJoin != 0)
				Client.RegisterInputSigChangeCallback(m_SubscribedAnalogModeJoin, eSigType.Analog, ModeChange);
		}

		/// <summary>
		/// Called when the press join state changes.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void ModeChange(object parent, SigAdapterEventArgs args)
		{
			if (AnalogModeJoin == 0)
				return;

			OnSetMode.Raise(this, new UShortEventArgs(args.Data.GetUShortValue()));
		}

		#endregion
	}
}
