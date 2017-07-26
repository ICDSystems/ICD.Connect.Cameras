using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.PanelClients;

namespace ICD.Connect.UI.ClientControls.Images
{
	[PublicAPI]
	public sealed class ClientImageObject : AbstractClientControl<ISigInputOutputClient>
	{
		[PublicAPI]
		public event EventHandler<StringEventArgs> OnSetImageUrl;

		[PublicAPI]
		public event EventHandler<UShortEventArgs> OnSetMode;

		private ushort m_AnalogModeJoin;
		private ushort m_SubscribedAnalogModeJoin;

		private ushort m_SerialImageUrlJoin;
		private ushort m_SubscribedSerialUrlJoin;

		#region Properties

		[PublicAPI]
		public ushort SerialGraphicsJoin
		{
			get { return m_SerialImageUrlJoin; }
			set
			{
				if (value == m_SerialImageUrlJoin)
					return;

				UnsubscribeSerialFeedback();
				m_SerialImageUrlJoin = value;
				SubscribeSerialFeedback();
			}
		}

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

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public ClientImageObject(ISigInputOutputClient client)
			: base(client)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		public ClientImageObject(ISigInputOutputClient client, IJoinOffsets parent)
			: base(client, parent)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public ClientImageObject(ISigInputOutputClient client, IJoinOffsets parent, ushort index)
			: base(client, parent, index)
		{
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribe to the panel events.
		/// </summary>
		protected override void SubscribeClientFeedback()
		{
			base.SubscribeClientFeedback();

			SubscribeAnalogFeedback();
			SubscribeSerialFeedback();
		}

		/// <summary>
		/// Unsubscribe from the panel events.
		/// </summary>
		protected override void UnsubscribeClientFeedback()
		{
			base.UnsubscribeClientFeedback();

			UnsubscribeAnalogFeedback();
			UnsubscribeSerialFeedback();
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
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeSerialFeedback()
		{
			if (m_SubscribedSerialUrlJoin != 0)
				Client.UnregisterInputSigChangeCallback(m_SubscribedSerialUrlJoin, eSigType.Serial, UrlChange);
			m_SubscribedSerialUrlJoin = 0;
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join and subscribes to the current join.
		/// </summary>
		private void SubscribeSerialFeedback()
		{
			UnsubscribeSerialFeedback();

			m_SubscribedSerialUrlJoin = Parent == null ? SerialGraphicsJoin : Parent.GetAnalogJoin(SerialGraphicsJoin, this);
			if (m_SubscribedSerialUrlJoin != 0)
				Client.RegisterInputSigChangeCallback(m_SubscribedSerialUrlJoin, eSigType.Serial, UrlChange);
		}

		/// <summary>
		/// Called when the mode join state changes.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void ModeChange(object parent, SigAdapterEventArgs args)
		{
			if (AnalogModeJoin == 0)
				return;

			OnSetMode.Raise(this, new UShortEventArgs(args.Data.GetUShortValue()));
		}

		private void UrlChange(object parent, SigAdapterEventArgs args)
		{
			if (SerialGraphicsJoin == 0)
				return;

			OnSetImageUrl.Raise(this, new StringEventArgs(args.Data.GetStringValue()));
		}

		#endregion
	}
}
