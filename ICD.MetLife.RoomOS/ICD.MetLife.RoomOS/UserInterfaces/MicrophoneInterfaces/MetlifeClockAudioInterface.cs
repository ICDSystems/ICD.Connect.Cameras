using ICD.Connect.Audio.ClockAudio;
using ICD.Common.EventArguments;

using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.MetLife.RoomOS.Rooms;

namespace ICD.MetLife.RoomOS.UserInterfaces.MicrophoneInterfaces
{
	/// <summary>
	/// The ClockAudioInterface simply handles the microphone button input,
	/// and changes the LEDs based on current conference state.
	/// </summary>
	public sealed class MetlifeClockAudioInterface : AbstractMicrophoneInterface
	{
		private readonly ClockAudioTs001Device m_Microphone;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="microphone"></param>
		public MetlifeClockAudioInterface(MetlifeRoom room, ClockAudioTs001Device microphone)
			: base(room)
		{
			m_Microphone = microphone;

			Subscribe(m_Microphone);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			Unsubscribe(m_Microphone);

			base.Dispose();
		}

		/// <summary>
		/// Sets the led colors based on call status.
		/// </summary>
		protected override void UpdateMicrophoneLeds()
		{
			bool greenLedEnabled = false;
			bool redLedEnabled = false;

			if (Room.ConferenceManager.IsInCall)
			{
				redLedEnabled = Room.ConferenceManager.ActiveConference.Status == eConferenceStatus.OnHold ||
								Room.ConferenceManager.PrivacyMuted;
				greenLedEnabled = !redLedEnabled;
			}

			m_Microphone.SetGreenLedEnabled(greenLedEnabled);
			m_Microphone.SetRedLedEnabled(redLedEnabled);
		}

		#region Device Callbacks

		/// <summary>
		/// Subscribes to the device events.
		/// </summary>
		/// <param name="microphone"></param>
		private void Subscribe(ClockAudioTs001Device microphone)
		{
			microphone.OnButtonPressedChanged += ClockAudioMicOnButtonPressedChanged;
		}

		/// <summary>
		/// Unsubscribes from the device events.
		/// </summary>
		/// <param name="microphone"></param>
		private void Unsubscribe(ClockAudioTs001Device microphone)
		{
			microphone.OnButtonPressedChanged -= ClockAudioMicOnButtonPressedChanged;
		}

		/// <summary>
		/// Called when a clock audio mic button is pressed/released.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ClockAudioMicOnButtonPressedChanged(object sender, BoolEventArgs args)
		{
			if (!args.Data)
				return;

			Room.ConferenceManager.TogglePrivacyMute();
		}

		#endregion
	}
}
