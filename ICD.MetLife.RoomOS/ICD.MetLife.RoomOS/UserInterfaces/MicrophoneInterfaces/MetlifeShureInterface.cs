using ICD.Connect.Audio.Shure;
using ICD.Connect.Conferencing.Conferences;

using ICD.MetLife.RoomOS.Rooms;

namespace ICD.MetLife.RoomOS.UserInterfaces.MicrophoneInterfaces
{
	/// <summary>
	/// The ClockAudioInterface simply handles the microphone button input,
	/// and changes the LEDs based on current conference state.
	/// </summary>
	public sealed class MetlifeShureInterface : AbstractMicrophoneInterface
	{
		private readonly IShureMxaDevice m_Microphone;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="microphone"></param>
		public MetlifeShureInterface(MetlifeRoom room, IShureMxaDevice microphone)
			: base(room)
		{
			m_Microphone = microphone;
		}

		/// <summary>
		/// Not in a call - LED Off
		/// In a call, On Hold - LED Yellow
		/// In a call, not on hold, muted - LED Red
		/// In a call, not on hold, not muted - LED Green
		/// </summary>
		protected override void UpdateMicrophoneLeds()
		{
			eLedBrightness brightness = eLedBrightness.Disabled;
			eLedColor color = eLedColor.White;

			if (Room.ConferenceManager.IsInCall)
			{
				brightness = eLedBrightness.Default;

				color = Room.ConferenceManager.ActiveConference.Status == eConferenceStatus.OnHold
					        ? eLedColor.Yellow
					        : Room.ConferenceManager.PrivacyMuted
						          ? eLedColor.Red
						          : eLedColor.Green;
			}

			m_Microphone.SetLedBrightness(brightness);
			m_Microphone.SetLedColor(color);
		}
	}
}
