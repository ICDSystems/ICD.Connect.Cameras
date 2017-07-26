using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.PanelClients;

namespace ICD.Connect.UI.ClientControls
{
	public abstract class AbstractClientControl<T> : IClientControl
		where T : class, ISigInputOutputClient
	{
		public event EventHandler<BoolEventArgs> OnSetEnabled;
		public event EventHandler<BoolEventArgs> OnSetVisible;

		private readonly T m_Client;
		private readonly IJoinOffsets m_Parent;
		private readonly ushort m_Index;

		private ushort m_DigitalVisibilityJoin;
		private ushort m_DigitalEnableJoin;

		private uint m_SubscribedDigitalVisibilityJoin;
		private uint m_SubscribedDigitalEnableJoin;

		#region Properties

		/// <summary>
		/// Gets the client this control is a part of.
		/// </summary>
		protected T Client { get { return m_Client; } }

		/// <summary>
		/// Gets/sets the digital visibility join.
		/// </summary>
		[PublicAPI]
		public ushort DigitalVisibilityJoin
		{
			get { return m_DigitalVisibilityJoin; }
			set
			{
				if (value == m_DigitalVisibilityJoin)
					return;

				UnsubscribeDigitalVisibilityFeedback();
				m_DigitalVisibilityJoin = value;
				SubscribeDigitalVisibilityFeedback();
			}
		}

		/// <summary>
		/// Gets/sets the digital enable join.
		/// </summary>
		[PublicAPI]
		public ushort DigitalEnableJoin
		{
			get { return m_DigitalEnableJoin; }
			set
			{
				if (value == m_DigitalEnableJoin)
					return;

				UnsubscribeDigitalEnableFeedback();
				m_DigitalEnableJoin = value;
				SubscribeDigitalEnableFeedback();
			}
		}

		/// <summary>
		/// Gets the index of the control in a list.
		/// </summary>
		public ushort Index { get { return m_Index; } }

		/// <summary>
		/// Gets/sets the parent for cumulative join offsets.
		/// </summary>
		public IJoinOffsets Parent { get { return m_Parent; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractClientControl(T client)
			: this(client, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		protected AbstractClientControl(T client, IJoinOffsets parent)
			: this(client, parent, 0)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractClientControl(T client, IJoinOffsets parent, ushort index)
		{
			if (client == null)
				throw new InvalidOperationException(string.Format("Creating {0} with null client", GetType().Name));

			m_Client = client;
			m_Parent = parent;
			m_Index = index;

			SubscribeClientFeedback();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			UnsubscribeClientFeedback();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Subscribe to the panel events.
		/// </summary>
		protected virtual void SubscribeClientFeedback()
		{
			SubscribeDigitalVisibilityFeedback();
			SubscribeDigitalEnableFeedback();
		}

		/// <summary>
		/// Unsubscribe from the panel events.
		/// </summary>
		protected virtual void UnsubscribeClientFeedback()
		{
			UnsubscribeDigitalVisibilityFeedback();
			UnsubscribeDigitalEnableFeedback();
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeDigitalVisibilityFeedback()
		{
			if (m_SubscribedDigitalVisibilityJoin != 0)
				m_Client.UnregisterInputSigChangeCallback(m_SubscribedDigitalVisibilityJoin, eSigType.Digital, VisibilityChange);
			m_SubscribedDigitalVisibilityJoin = 0;
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join and subscribes to the current join.
		/// </summary>
		private void SubscribeDigitalVisibilityFeedback()
		{
			UnsubscribeDigitalVisibilityFeedback();

			m_SubscribedDigitalVisibilityJoin = Parent == null ? DigitalVisibilityJoin : Parent.GetDigitalJoin(DigitalVisibilityJoin, this);
			if (m_SubscribedDigitalVisibilityJoin != 0)
				m_Client.RegisterInputSigChangeCallback(m_SubscribedDigitalVisibilityJoin, eSigType.Digital, VisibilityChange);
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeDigitalEnableFeedback()
		{
			if (m_SubscribedDigitalEnableJoin != 0)
				m_Client.UnregisterInputSigChangeCallback(m_SubscribedDigitalEnableJoin, eSigType.Digital, EnableChange);
			m_SubscribedDigitalEnableJoin = 0;
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join and subscribes to the current join.
		/// </summary>
		private void SubscribeDigitalEnableFeedback()
		{
			UnsubscribeDigitalVisibilityFeedback();

			m_SubscribedDigitalEnableJoin = Parent == null ? DigitalEnableJoin : Parent.GetDigitalJoin(DigitalEnableJoin, this);
			if (m_SubscribedDigitalVisibilityJoin != 0)
				m_Client.RegisterInputSigChangeCallback(m_SubscribedDigitalEnableJoin, eSigType.Digital, VisibilityChange);
		}

		private void EnableChange(object parent, SigAdapterEventArgs args)
		{
			if (DigitalEnableJoin == 0)
				return;

			OnSetEnabled.Raise(this, new BoolEventArgs(args.Data.GetBoolValue()));
		}

		private void VisibilityChange(object parent, SigAdapterEventArgs args)
		{
			if (DigitalVisibilityJoin == 0)
				return;

			OnSetVisible.Raise(this, new BoolEventArgs(args.Data.GetBoolValue()));
		}

		#endregion
	}
}
