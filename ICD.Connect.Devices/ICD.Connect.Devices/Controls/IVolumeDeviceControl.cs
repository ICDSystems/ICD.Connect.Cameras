using System;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Utils;

namespace ICD.Connect.Devices.Controls
{
	/// <summary>
	/// IVolumeDeviceControls are used with devices that have volume and mute features.
	/// For example, a TV or a Codec.
	/// </summary>
	public interface IVolumeDeviceControl : IDeviceControl
	{
		/// <summary>
		/// Raised when the raw volume changes.
		/// </summary>
		[PublicAPI]
		event EventHandler<FloatEventArgs> OnRawVolumeChanged;

		/// <summary>
		/// Raised when the mute state changes.
		/// </summary>
		[PublicAPI]
		event EventHandler<BoolEventArgs> OnMuteStateChanged;

		#region Properties

		/// <summary>
		/// Gets the current volume.
		/// </summary>
		[PublicAPI]
		float RawVolume { get; }

		/// <summary>
		/// The min volume.
		/// </summary>
		[PublicAPI]
		float RawVolumeMin { get; }

		/// <summary>
		/// The max volume.
		/// </summary>
		[PublicAPI]
		float RawVolumeMax { get; }

		/// <summary>
		/// Prevents the control from going below this volume.
		/// </summary>
		[PublicAPI]
		float? RawVolumeSafetyMin { get; set; }

		/// <summary>
		/// Prevents the control from going above this volume.
		/// </summary>
		[PublicAPI]
		float? RawVolumeSafetyMax { get; set; }

		/// <summary>
		/// The volume the control is set to when the device comes online.
		/// </summary>
		[PublicAPI]
		float? RawVolumeDefault { get; set; }

		/// <summary>
		/// Gets the muted state.
		/// </summary>
		[PublicAPI]
		bool IsMuted { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Sets the raw volume. This will be clamped to the min/max and safety min/max.
		/// </summary>
		/// <param name="volume"></param>
		[PublicAPI]
		void SetRawVolume(float volume);

		/// <summary>
		/// Sets the mute state.
		/// </summary>
		/// <param name="mute"></param>
		[PublicAPI]
		void SetMute(bool mute);

		/// <summary>
		/// Increments the raw volume once.
		/// </summary>
		[PublicAPI]
		void RawVolumeIncrement();

		/// <summary>
		/// Decrements the raw volume once.
		/// </summary>
		[PublicAPI]
		void RawVolumeDecrement();

		#endregion
	}

	public static class DeviceVolumeControlExtensions
	{
		/// <summary>
		/// Enables mute.
		/// </summary>
		/// <param name="extends"></param>
		[PublicAPI]
		public static void MuteOn(this IVolumeDeviceControl extends)
		{
			extends.SetMute(true);
		}

		/// <summary>
		/// Disables mute.
		/// </summary>
		/// <param name="extends"></param>
		[PublicAPI]
		public static void MuteOff(this IVolumeDeviceControl extends)
		{
			extends.SetMute(false);
		}

		/// <summary>
		/// Toggles the current mute state.
		/// </summary>
		/// <param name="extends"></param>
		[PublicAPI]
		public static void MuteToggle(this IVolumeDeviceControl extends)
		{
			extends.SetMute(!extends.IsMuted);
		}

		/// <summary>
		/// Returns the volume in the range 0.0 - 1.0 bewteen volume min and max.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static float GetRawVolumeAsPercentage(this IVolumeDeviceControl extends)
		{
			return MathUtils.MapRange(extends.RawVolumeMin, extends.RawVolumeMax, 0.0f, 1.0f, extends.RawVolume);
		}

		/// <summary>
		/// Returns the volume in the range 0.0 - 1.0 between safety min and max.
		/// If no safety min/max has been configured, uses the control min/max.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static float GetRawVolumeAsSafetyPercentage(this IVolumeDeviceControl extends)
		{
			float min = extends.RawVolumeSafetyMin ?? extends.RawVolumeMin;
			float max = extends.RawVolumeSafetyMax ?? extends.RawVolumeMax;

			return MathUtils.MapRange(min, max, 0.0f, 1.0f, extends.RawVolume);
		}

		/// <summary>
		/// Returns the larger of the control volume min or the safety volume min.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static float GetRawVolumeMinOrSafetyMin(this IVolumeDeviceControl extends)
		{
			float min = extends.RawVolumeMin;
			float safetyMin = extends.RawVolumeSafetyMin ?? min;

			return Math.Max(min, safetyMin);
		}

		/// <summary>
		/// Returns the smaller of the control volume max or the safety volume max.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[PublicAPI]
		public static float GetRawVolumeMaxOrSafetyMax(this IVolumeDeviceControl extends)
		{
			float max = extends.RawVolumeMax;
			float safetyMax = extends.RawVolumeSafetyMax ?? max;

			return Math.Min(max, safetyMax);
		}
	}
}
