using System;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Settings.Core;

namespace ICD.MetLife.RoomOS.Endpoints.Destinations
{
	public sealed class MetlifeDestination : AbstractDestination<MetlifeDestinationSettings>
	{
		public enum eVtcOption
		{
			None,

			/// <summary>
			/// The far end contacts.
			/// </summary>
			Main,

			/// <summary>
			/// Presentation/SelfView
			/// </summary>
			Secondary,

			/// <summary>
			/// Presentation.
			/// </summary>
			ContentOnly
		}

		[Flags]
		public enum eAudioOption
		{
			None,
			Program,
			Call
		}

		/// <summary>
		/// Describes which VTC source is routed to this destination.
		/// </summary>
		public eVtcOption VtcOption { get; set; }

		/// <summary>
		/// Describes which Audio source is routed to this destination.
		/// </summary>
		public eAudioOption AudioOption { get; set; }

		/// <summary>
		/// Determines if a source is automatically routed to this destination when the share button is
		/// pressed, or the source is detected by the system.
		/// </summary>
		public bool ShareByDefault { get; set; }

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			VtcOption = eVtcOption.Main;
			AudioOption = eAudioOption.Program | eAudioOption.Call;
			ShareByDefault = false;
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(MetlifeDestinationSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.VtcOption = VtcOption;
			settings.AudioOption = AudioOption;
			settings.ShareByDefault = ShareByDefault;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(MetlifeDestinationSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			VtcOption = settings.VtcOption;
			AudioOption = settings.AudioOption;
			ShareByDefault = settings.ShareByDefault;
		}

		#endregion
	}
}
