using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Timers;
using ICD.Connect.Panels;

namespace ICD.Connect.UI.Controls
{
	/// <summary>
	/// Represents a VTPro sound effect that can be played/stopped at runtime.
	/// </summary>
	public sealed class VtProSound : IDisposable
	{
		private readonly IPanelDevice m_Panel;
		private readonly SafeTimer m_LoopTimer;
		private long m_LoopInterval;

		#region Properties

		[PublicAPI]
		public ushort JoinNumber { get; set; }

		[PublicAPI]
		public ushort StopSoundJoin { get; set; }

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="panel"></param>
		public VtProSound(IPanelDevice panel)
		{
			if (panel == null)
				throw new InvalidOperationException(string.Format("Creating {0} with null panel", GetType().Name));

			m_LoopTimer = SafeTimer.Stopped(LoopTimerCallback);
			m_Panel = panel;
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_LoopTimer.Dispose();
		}

		/// <summary>
		/// Starts the sound.
		/// </summary>
		[PublicAPI]
		public void Play()
		{
			Play(0);
		}

		/// <summary>
		/// Starts the sound.
		/// </summary>
		/// <param name="loopInterval">How often to loop in milliseconds, no loop if 0</param>
		public void Play(long loopInterval)
		{
			m_Panel.SendInputDigital(JoinNumber, true);
			m_Panel.SendInputDigital(JoinNumber, false);

			if (loopInterval > 0)
				ResetTimer(loopInterval);
			else
				StopTimer();
		}

		/// <summary>
		/// Stops the sound.
		/// </summary>
		public void Stop()
		{
			m_Panel.SendInputDigital(StopSoundJoin, true);
			m_Panel.SendInputDigital(StopSoundJoin, false);

			StopTimer();
		}

		#endregion

		#region Private Methods

		private void ResetTimer(long loopInterval)
		{
			m_LoopInterval = loopInterval;
			m_LoopTimer.Reset(m_LoopInterval);
		}

		private void StopTimer()
		{
			m_LoopTimer.Stop();
		}

		private void LoopTimerCallback()
		{
			// Protection against Crestron disposing the timer.
			if (m_Panel == null)
				return;

			Play(m_LoopInterval);
		}

		#endregion
	}
}
