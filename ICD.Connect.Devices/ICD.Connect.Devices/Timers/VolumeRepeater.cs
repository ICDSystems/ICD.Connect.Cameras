using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Devices.Timers
{
	/// <summary>
	/// VolumeRepeater allows for a virtual "button" to be held, raising a callback for
	/// every repeat interval.
	/// </summary>
	public sealed class VolumeRepeater : IDisposable
	{
		private readonly SafeTimer m_RepeatTimer;

		private IVolumeDeviceControl m_Control;

		private readonly int m_BeforeRepeat;
		private readonly int m_BetweenRepeat;
		private readonly int m_InitialIncrement;
		private readonly int m_RepeatIncrement;

		private bool m_Up;

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="initialIncrement">The increment when the repeater is first held</param>
		/// <param name="repeatIncrement">The increment for every subsequent repeat</param>
		/// <param name="beforeRepeat">The delay before the second increment</param>
		/// <param name="betweenRepeat">The delay between each subsequent repeat</param>
		public VolumeRepeater(int initialIncrement, int repeatIncrement, int beforeRepeat, int betweenRepeat)
		{
			m_RepeatTimer = SafeTimer.Stopped(RepeatCallback);

			m_InitialIncrement = initialIncrement;
			m_RepeatIncrement = repeatIncrement;
			m_BeforeRepeat = beforeRepeat;
			m_BetweenRepeat = betweenRepeat;
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~VolumeRepeater()
		{
			Dispose();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_RepeatTimer.Dispose();
		}

		/// <summary>
		/// Sets the control.
		/// </summary>
		/// <param name="control"></param>
		public void SetControl(IVolumeDeviceControl control)
		{
			m_Control = control;
		}

		/// <summary>
		/// Begin incrementing the volume.
		/// </summary>
		public void VolumeUpHold()
		{
			Release();
			BeginIncrement(true);
		}

		/// <summary>
		/// Begin decrementing the volume.
		/// </summary>
		public void VolumeDownHold()
		{
			Release();
			BeginIncrement(false);
		}

		/// <summary>
		/// Stops the repeat timer.
		/// </summary>
		public void Release()
		{
			m_RepeatTimer.Stop();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Applies the initial increment and resets the timer.
		/// </summary>
		/// <param name="up"></param>
		private void BeginIncrement(bool up)
		{
			m_Up = up;

			IncrementVolume(m_InitialIncrement);

			m_RepeatTimer.Reset(m_BeforeRepeat, m_BetweenRepeat);
		}

		/// <summary>
		/// Called for every repeat.
		/// </summary>
		private void RepeatCallback()
		{
			IncrementVolume(m_RepeatIncrement);
		}

		/// <summary>
		/// Adjusts the device volume.
		/// </summary>
		/// <param name="increment"></param>
		private void IncrementVolume(int increment)
		{
			int delta = m_Up ? increment : -1 * increment;
			float newVolume =
				MathUtils.Clamp(m_Control.RawVolume + delta, m_Control.RawVolumeMin, m_Control.RawVolumeMax);

			m_Control.SetRawVolume(newVolume);
		}

		#endregion
	}
}
