using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.PanelClients;

namespace ICD.Connect.UI.ClientControls.TextControls
{
	public abstract class AbstractClientLabel : AbstractClientControl<ISigInputOutputClient>
	{
		public delegate void SetDigitalTextAtJoinCallback(AbstractClientLabel sender, ushort join, bool digital);

		public delegate void SetAnalogTextAtJoinCallback(AbstractClientLabel sender, ushort join, ushort analog);

		public delegate void SetSerialTextAtJoinCallback(AbstractClientLabel sender, ushort join, string serial);

		[PublicAPI]
		public event EventHandler<StringEventArgs> OnSetLabelText;

		[PublicAPI]
		public event SetDigitalTextAtJoinCallback OnSetDigitalTextAtJoin;

		[PublicAPI]
		public event SetAnalogTextAtJoinCallback OnSetAnalogLabelTextAtJoin;

		[PublicAPI]
		public event SetSerialTextAtJoinCallback OnSetSerialLabelTextAtJoin;

		private IcdHashSet<ushort> m_DigitalJoins;
		private IcdHashSet<ushort> m_AnalogJoins;
		private IcdHashSet<ushort> m_SerialJoins;

		private IcdHashSet<ushort> m_SubscribedDigitalJoins;
		private IcdHashSet<ushort> m_SubscribedAnalogJoins;
		private IcdHashSet<ushort> m_SubscribedSerialJoins;

		private ushort m_IndirectTextJoin;
		private ushort m_SubscribedIndirectTextJoin;

		#region Properties

		[PublicAPI]
		public ushort IndirectTextJoin
		{
			get { return m_IndirectTextJoin; }
			set
			{
				if (value == m_IndirectTextJoin)
					return;

				UnsubscribeIndirectTextFeedback();
				m_IndirectTextJoin = value;
				SubscribeIndirectTextFeedback();
			}
		}

		private IcdHashSet<ushort> DigitalJoins { get { return m_DigitalJoins ?? (m_DigitalJoins = new IcdHashSet<ushort>()); } }

		private IcdHashSet<ushort> AnalogJoins { get { return m_AnalogJoins ?? (m_AnalogJoins = new IcdHashSet<ushort>()); } }

		private IcdHashSet<ushort> SerialJoins { get { return m_SerialJoins ?? (m_SerialJoins = new IcdHashSet<ushort>()); } }

		private IcdHashSet<ushort> SubscribedDigitalJoins
		{
			get { return m_SubscribedDigitalJoins ?? (m_SubscribedDigitalJoins = new IcdHashSet<ushort>()); }
		}

		private IcdHashSet<ushort> SubscribedAnalogJoins
		{
			get { return m_SubscribedAnalogJoins ?? (m_SubscribedAnalogJoins = new IcdHashSet<ushort>()); }
		}

		private IcdHashSet<ushort> SubscribedSerialJoins
		{
			get { return m_SubscribedSerialJoins ?? (m_SubscribedSerialJoins = new IcdHashSet<ushort>()); }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractClientLabel(ISigInputOutputClient client)
			: base(client)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		protected AbstractClientLabel(ISigInputOutputClient client, IJoinOffsets parent)
			: base(client, parent)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractClientLabel(ISigInputOutputClient client, IJoinOffsets parent, ushort index)
			: base(client, parent, index)
		{
		}

		#endregion

		#region Methods

		public void SetDigitalJoins(IEnumerable<ushort> joins)
		{
			UnsubscribeDigitalFeedback();

			AnalogJoins.Clear();
			AnalogJoins.AddRange(joins);

			SubscribeDigitalFeedback();
		}

		public void SetAnalogJoins(IEnumerable<ushort> joins)
		{
			UnsubscribeAnalogFeedback();

			AnalogJoins.Clear();
			AnalogJoins.AddRange(joins);

			SubscribeAnalogFeedback();
		}

		public void SetSerialJoins(IEnumerable<ushort> joins)
		{
			UnsubscribeSerialFeedback();

			SerialJoins.Clear();
			SerialJoins.AddRange(joins);

			SubscribeSerialFeedback();
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
			SubscribeDigitalFeedback();
			SubscribeIndirectTextFeedback();
		}

		/// <summary>
		/// Unsubscribe from the panel events.
		/// </summary>
		protected override void UnsubscribeClientFeedback()
		{
			base.UnsubscribeClientFeedback();

			UnsubscribeAnalogFeedback();
			UnsubscribeSerialFeedback();
			UnsubscribeDigitalFeedback();
			UnsubscribeIndirectTextFeedback();
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeIndirectTextFeedback()
		{
			if (m_SubscribedIndirectTextJoin != 0)
				Client.UnregisterInputSigChangeCallback(m_SubscribedIndirectTextJoin, eSigType.Serial, TextChange);
			m_SubscribedIndirectTextJoin = 0;
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join and subscribes to the current join.
		/// </summary>
		private void SubscribeIndirectTextFeedback()
		{
			UnsubscribeIndirectTextFeedback();

			m_SubscribedIndirectTextJoin = Parent == null ? m_IndirectTextJoin : Parent.GetAnalogJoin(m_IndirectTextJoin, this);
			if (m_SubscribedIndirectTextJoin != 0)
				Client.RegisterInputSigChangeCallback(m_SubscribedIndirectTextJoin, eSigType.Serial, TextChange);
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeAnalogFeedback()
		{
			foreach (ushort join in SubscribedAnalogJoins)
				Client.UnregisterInputSigChangeCallback(join, eSigType.Analog, AnalogJoinChange);
			SubscribedAnalogJoins.Clear();
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join and subscribes to the current join.
		/// </summary>
		private void SubscribeAnalogFeedback()
		{
			UnsubscribeAnalogFeedback();

			foreach (ushort join in AnalogJoins)
			{
				ushort offsetJoin = Parent == null ? join : Parent.GetAnalogJoin(join, this);
				if (offsetJoin != 0)
					Client.RegisterInputSigChangeCallback(offsetJoin, eSigType.Analog, AnalogJoinChange);
				SubscribedAnalogJoins.Add(offsetJoin);
			}
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeDigitalFeedback()
		{
			foreach (ushort join in SubscribedDigitalJoins)
				Client.UnregisterInputSigChangeCallback(join, eSigType.Digital, DigitalJoinChange);
			SubscribedDigitalJoins.Clear();
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join and subscribes to the current join.
		/// </summary>
		private void SubscribeDigitalFeedback()
		{
			UnsubscribeDigitalFeedback();

			foreach (ushort join in DigitalJoins)
			{
				ushort offsetJoin = Parent == null ? join : Parent.GetDigitalJoin(join, this);
				if (offsetJoin != 0)
					Client.RegisterInputSigChangeCallback(offsetJoin, eSigType.Digital, DigitalJoinChange);
				SubscribedDigitalJoins.Add(offsetJoin);
			}
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeSerialFeedback()
		{
			foreach (ushort join in SubscribedSerialJoins)
				Client.UnregisterInputSigChangeCallback(join, eSigType.Serial, SerialJoinChange);
			SubscribedSerialJoins.Clear();
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join and subscribes to the current join.
		/// </summary>
		private void SubscribeSerialFeedback()
		{
			UnsubscribeSerialFeedback();

			foreach (ushort join in SerialJoins)
			{
				ushort offsetJoin = Parent == null ? join : Parent.GetSerialJoin(join, this);
				if (offsetJoin != 0)
					Client.RegisterInputSigChangeCallback(offsetJoin, eSigType.Serial, SerialJoinChange);
				SubscribedSerialJoins.Add(offsetJoin);
			}
		}

		private void TextChange(object parent, SigAdapterEventArgs args)
		{
			if (args.Data.Number == 0)
				return;

			OnSetLabelText.Raise(this, new StringEventArgs(args.Data.GetStringValue()));
		}

		private void AnalogJoinChange(object parent, SigAdapterEventArgs args)
		{
			if (args.Data.Number == 0)
				return;

			SetAnalogTextAtJoinCallback handler = OnSetAnalogLabelTextAtJoin;
			if (handler != null)
				handler(this, (ushort)args.Data.Number, args.Data.GetUShortValue());
		}

		private void DigitalJoinChange(object parent, SigAdapterEventArgs args)
		{
			if (args.Data.Number == 0)
				return;

			SetDigitalTextAtJoinCallback handler = OnSetDigitalTextAtJoin;
			if (handler != null)
				handler(this, (ushort)args.Data.Number, args.Data.GetBoolValue());
		}

		private void SerialJoinChange(object parent, SigAdapterEventArgs args)
		{
			if (args.Data.Number == 0)
				return;

			SetSerialTextAtJoinCallback handler = OnSetSerialLabelTextAtJoin;
			if (handler != null)
				handler(this, (ushort)args.Data.Number, args.Data.GetStringValue());
		}

		#endregion
	}
}
