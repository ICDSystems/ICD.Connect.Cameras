using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Core;

namespace ICD.MetLife.RoomOS.Endpoints.Sources
{
	public sealed class MetlifeSource : AbstractSource<MetlifeSourceSettings>
	{
		#region Properties

		/// <summary>
		/// Type of source, mainly for icon
		/// </summary>
		public eSourceType SourceType { get; set; }

		/// <summary>
		/// Flags to determine where this source is displayed
		/// </summary>
		public eSourceFlags SourceFlags { get; set; }

		/// <summary>
		/// Determines the detection state of the source in the UI.
		/// Typically a device is only available to share when it is actively transmitting on the given output.
		/// Some devices (Clickshare) use this property to enable sharing even if the device is only transmitting
		/// a blank signal (no buttons transmitting). 
		/// </summary>
		public bool EnableWhenNotTransmitting { get; set; }

		/// <summary>
		/// When true the source is not considered for automatic routing upon detection by the system.
		/// </summary>
		public bool InhibitAutoRoute { get; set; }

		/// <summary>
		/// When true the system will not automatically unroute the source when a signal is no longer detected.
		/// </summary>
		public bool InhibitAutoUnroute { get; set; }

		#endregion

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SourceType = eSourceType.Laptop;
			SourceFlags = eSourceFlags.Share;
			EnableWhenNotTransmitting = false;
			InhibitAutoRoute = false;
			InhibitAutoUnroute = false;
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(MetlifeSourceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.SourceType = SourceType;
			settings.SourceFlags = SourceFlags;
			settings.EnableWhenNotTransmitting = EnableWhenNotTransmitting;
			settings.InhibitAutoRoute = InhibitAutoRoute;
			settings.InhibitAutoUnroute = InhibitAutoUnroute;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(MetlifeSourceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			SourceType = settings.SourceType;
			SourceFlags = settings.SourceFlags;
			EnableWhenNotTransmitting = settings.EnableWhenNotTransmitting;
			InhibitAutoRoute = settings.InhibitAutoRoute;
			InhibitAutoUnroute = settings.InhibitAutoUnroute;
		}

		#endregion
	}
}
