using System;
using ICD.Common.Properties;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.SigIo
{
	public abstract class AbstractSigOutput : IDisposable
	{
		private readonly ISigDevice m_Device;
		private uint m_Number;

		#region Properties

		/// <summary>
		/// Gets the device.
		/// </summary>
		protected ISigDevice Device { get { return m_Device; } }

		/// <summary>
		/// Gets/sets the sig number to subscribe to.
		/// </summary>
		[PublicAPI]
		public uint Number
		{
			get { return m_Number; }
			set
			{
				if (value == m_Number)
					return;

				Unsubscribe(m_Number);
				m_Number = value;
				Subscribe(m_Number);
			}
		}

		/// <summary>
		/// Gets the sigtype we are interested in.
		/// </summary>
		protected abstract eSigType SigType { get; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		protected AbstractSigOutput(ISigDevice device)
			: this(device, 0)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="number"></param>
		protected AbstractSigOutput(ISigDevice device, uint number)
		{
			m_Device = device;
			Number = number;
		}

		#endregion

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			Unsubscribe(m_Number);
		}

		#region Private Methods

		/// <summary>
		/// Subscribe to the device output with the given number.
		/// </summary>
		/// <param name="number"></param>
		private void Subscribe(uint number)
		{
			if (number != 0)
				m_Device.RegisterOutputSigChangeCallback(number, SigType, DeviceOnOutput);
		}

		/// <summary>
		/// Unsubscribe to the device output with the given number.
		/// </summary>
		/// <param name="number"></param>
		private void Unsubscribe(uint number)
		{
			if (number != 0)
				m_Device.UnregisterOutputSigChangeCallback(number, SigType, DeviceOnOutput);
		}

		/// <summary>
		/// Called when a subscribed output changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected abstract void DeviceOnOutput(SigCallbackManager sender, SigAdapterEventArgs args);

		#endregion
	}
}
