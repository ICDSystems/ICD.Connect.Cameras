using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;
using ICD.Connect.UI.ClientControls.TextControls;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.PanelClients;

namespace ICD.Connect.UI.ClientControls.Buttons
{
	public abstract class AbstractClientButton : AbstractClientLabel
	{
		[PublicAPI]
		public event EventHandler<BoolEventArgs> OnSetSelected;

		private ushort m_DigitalPressJoin;
		private ushort m_SubscribedDigitalPressJoin;

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

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractClientButton(ISigInputOutputClient client)
			: base(client)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		protected AbstractClientButton(ISigInputOutputClient client, IJoinOffsets parent)
			: base(client, parent)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected AbstractClientButton(ISigInputOutputClient client, IJoinOffsets parent, ushort index)
			: base(client, parent, index)
		{
		}

		/// <summary>
		/// Sends the press event to the server.
		/// </summary>
		[PublicAPI]
		public void Press()
		{
			Client.SendOutputDigital(DigitalPressJoin, true);
		}

		/// <summary>
		/// Sends the release event to the server.
		/// </summary>
		[PublicAPI]
		public void Release()
		{
			Client.SendOutputDigital(DigitalPressJoin, false);
		}

		#region Private Methods

		/// <summary>
		/// Subscribe to the panel events.
		/// </summary>
		protected override void SubscribeClientFeedback()
		{
			base.SubscribeClientFeedback();

			SubscribeDigitalFeedback();
		}

		/// <summary>
		/// Unsubscribe from the panel events.
		/// </summary>
		protected override void UnsubscribeClientFeedback()
		{
			base.UnsubscribeClientFeedback();

			UnsubscribeDigitalFeedback();
		}

		/// <summary>
		/// Unsubscribes from the previous subscribed join.
		/// </summary>
		private void UnsubscribeDigitalFeedback()
		{
			if (m_SubscribedDigitalPressJoin != 0)
				Client.UnregisterInputSigChangeCallback(m_SubscribedDigitalPressJoin, eSigType.Digital, SelectionChange);
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
				Client.RegisterInputSigChangeCallback(m_SubscribedDigitalPressJoin, eSigType.Digital, SelectionChange);
		}

		/// <summary>
		/// Called when the press join state changes.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="args"></param>
		private void SelectionChange(object parent, SigAdapterEventArgs args)
		{
			if (DigitalPressJoin == 0)
				return;

			OnSetSelected.Raise(this, new BoolEventArgs(args.Data.GetBoolValue()));
		}

		#endregion
	}
}
