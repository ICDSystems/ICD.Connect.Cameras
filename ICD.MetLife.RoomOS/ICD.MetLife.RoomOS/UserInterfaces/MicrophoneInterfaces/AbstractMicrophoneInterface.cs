using System;
using ICD.Common.EventArguments;

using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.EventArguments;
using ICD.MetLife.RoomOS.Rooms;

namespace ICD.MetLife.RoomOS.UserInterfaces.MicrophoneInterfaces
{
	public abstract class AbstractMicrophoneInterface : IDisposable
	{
		private readonly MetlifeRoom m_Room;

		protected MetlifeRoom Room { get { return m_Room; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		protected AbstractMicrophoneInterface(MetlifeRoom room)
		{
			m_Room = room;
			Subscribe(m_Room);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			Unsubscribe(m_Room);
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="metlifeRoom"></param>
		private void Subscribe(MetlifeRoom metlifeRoom)
		{
			metlifeRoom.ConferenceManager.OnInCallChanged += ConferenceManagerOnInCallChanged;
			metlifeRoom.ConferenceManager.OnRecentConferenceAdded += ConferenceManagerOnRecentConferenceAdded;
			metlifeRoom.ConferenceManager.OnActiveConferenceStatusChanged += ConferenceManagerOnActiveConferenceStatusChanged;
			metlifeRoom.ConferenceManager.OnPrivacyMuteStatusChange += ConferenceManagerOnPrivacyMuteStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="metlifeRoom"></param>
		private void Unsubscribe(MetlifeRoom metlifeRoom)
		{
			metlifeRoom.ConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;
			metlifeRoom.ConferenceManager.OnRecentConferenceAdded -= ConferenceManagerOnRecentConferenceAdded;
			metlifeRoom.ConferenceManager.OnActiveConferenceStatusChanged -= ConferenceManagerOnActiveConferenceStatusChanged;
			metlifeRoom.ConferenceManager.OnPrivacyMuteStatusChange -= ConferenceManagerOnPrivacyMuteStatusChange;
		}

		/// <summary>
		/// Called when the active conference changes status.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnActiveConferenceStatusChanged(object sender, ConferenceStatusEventArgs args)
		{
			UpdateMicrophoneLeds();
		}

		/// <summary>
		/// Called when a conference is added to the conference manager.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnRecentConferenceAdded(object sender, ConferenceEventArgs args)
		{
			UpdateMicrophoneLeds();
		}

		/// <summary>
		/// Called when we enable/disable privacy mute.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnPrivacyMuteStatusChange(object sender, BoolEventArgs args)
		{
			UpdateMicrophoneLeds();
		}

		/// <summary>
		/// Called when we enter a call, or leave all calls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnInCallChanged(object sender, BoolEventArgs args)
		{
			UpdateMicrophoneLeds();
		}

		/// <summary>
		/// Sets the led colors based on call status.
		/// </summary>
		protected abstract void UpdateMicrophoneLeds();

		#endregion
	}
}
