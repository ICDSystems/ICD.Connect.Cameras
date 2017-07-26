using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Devices.Controls
{
	/// <summary>
	/// Base class for volume device controls.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AbstractVolumeDeviceControl<T> : AbstractDeviceControl<T>, IVolumeDeviceControl
		where T : IDevice
	{
		public event EventHandler<FloatEventArgs> OnRawVolumeChanged;
		public event EventHandler<BoolEventArgs> OnMuteStateChanged;

		private float m_RawVolume;
		private bool m_IsMuted;
		private float? m_RawVolumeSafetyMin;
		private float? m_RawVolumeSafetyMax;

		#region Properties

		/// <summary>
		/// Gets the current volume.
		/// </summary>
		public float RawVolume
		{
			get { return m_RawVolume; }
			protected set
			{
				if (value == m_RawVolume)
					return;

				m_RawVolume = value;

				OnRawVolumeChanged.Raise(this, new FloatEventArgs(m_RawVolume));

				// Ensure the volume is in the min/max range.
				ClampVolume();
			}
		}

		/// <summary>
		/// The min volume.
		/// </summary>
		public abstract float RawVolumeMin { get; }

		/// <summary>
		/// The max volume.
		/// </summary>
		public abstract float RawVolumeMax { get; }

		/// <summary>
		/// Prevents the control from going below this volume.
		/// </summary>
		public float? RawVolumeSafetyMin
		{
			get { return m_RawVolumeSafetyMin; }
			set
			{
				if (value == m_RawVolumeSafetyMin)
					return;

				m_RawVolumeSafetyMin = value;

				// Ensure the volume is in the min/max range.
				ClampVolume();
			}
		}

		/// <summary>
		/// Prevents the control from going above this volume.
		/// </summary>
		public float? RawVolumeSafetyMax
		{
			get { return m_RawVolumeSafetyMax; }
			set
			{
				if (value == m_RawVolumeSafetyMax)
					return;

				m_RawVolumeSafetyMax = value;

				// Ensure the volume is in the min/max range.
				ClampVolume();
			}
		}

		/// <summary>
		/// The volume the control is set to when the device comes online.
		/// </summary>
		public abstract float? RawVolumeDefault { get; set; }

		/// <summary>
		/// Gets the muted state.
		/// </summary>
		public bool IsMuted
		{
			get { return m_IsMuted; }
			protected set
			{
				if (value == m_IsMuted)
					return;

				m_IsMuted = value;

				OnMuteStateChanged.Raise(this, new BoolEventArgs(m_IsMuted));
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		protected AbstractVolumeDeviceControl(T parent, int id)
			: base(parent, id)
		{
		}

		#region Methods

		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			OnRawVolumeChanged = null;
			OnMuteStateChanged = null;

			base.DisposeFinal(disposing);
		}

		/// <summary>
		/// Sets the raw volume. This will be clamped to the min/max and safety min/max.
		/// </summary>
		/// <param name="volume"></param>
		public abstract void SetRawVolume(float volume);

		/// <summary>
		/// Sets the mute state.
		/// </summary>
		/// <param name="mute"></param>
		public abstract void SetMute(bool mute);

		/// <summary>
		/// Increments the raw volume once.
		/// </summary>
		public abstract void RawVolumeIncrement();

		/// <summary>
		/// Decrements the raw volume once.
		/// </summary>
		public abstract void RawVolumeDecrement();

		#endregion

		/// <summary>
		/// Ensures the volume is in the correct range. If not, sets the volume to a clamped value.
		/// </summary>
		private void ClampVolume()
		{
			float min = this.GetRawVolumeMinOrSafetyMin();
			float max = this.GetRawVolumeMaxOrSafetyMax();

			if (RawVolume > min && RawVolume < max)
				return;

			float safeVolume = MathUtils.Clamp(RawVolume, min, max);
			SetRawVolume(safeVolume);
		}

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			float volume = this.GetRawVolumeAsPercentage() * 100;
			string percentage = string.Format("{0}%", (int)volume);

			addRow("Muted", IsMuted);
			addRow("Volume", percentage);
			addRow("Device volume range", string.Format("{0} - {1}", RawVolumeMin, RawVolumeMax));
			addRow("Safety volume range", string.Format("{0} - {1}", RawVolumeSafetyMin, RawVolumeSafetyMax));
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("MuteOn", "Mutes the audio", () => this.MuteOn());
			yield return new ConsoleCommand("MuteOff", "Unmutes the audio", () => this.MuteOff());
			yield return new ConsoleCommand("MuteToggle", "Toggles the audio mute state", () => this.MuteToggle());

			string setVolumeHelp = string.Format("SetVolume <{0}>",
			                                     StringUtils.RangeFormat(this.GetRawVolumeMinOrSafetyMin(),
			                                                             this.GetRawVolumeMaxOrSafetyMax()));
			yield return new GenericConsoleCommand<float>("SetVolume", setVolumeHelp, a => SetRawVolume(a));

			string setSafetyMinVolumeHelp = string.Format("SetSafetyMinVolume <{0}>",
			                                              StringUtils.RangeFormat(RawVolumeMin, RawVolumeMax));
			yield return
				new GenericConsoleCommand<float>("SetSafetyMinVolume", setSafetyMinVolumeHelp, v => RawVolumeSafetyMin = v);
			yield return new ConsoleCommand("ClearSafetyMinVolume", "", () => RawVolumeSafetyMin = null);

			string setSafetyMaxVolumeHelp = string.Format("SetSafetyMaxVolume <{0}>",
			                                              StringUtils.RangeFormat(RawVolumeMin, RawVolumeMax));
			yield return
				new GenericConsoleCommand<float>("SetSafetyMaxVolume", setSafetyMaxVolumeHelp, v => RawVolumeSafetyMax = v);
			yield return new ConsoleCommand("ClearSafetyMaxVolume", "", () => RawVolumeSafetyMax = null);
		}

		/// <summary>
		/// Workaround for the "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
