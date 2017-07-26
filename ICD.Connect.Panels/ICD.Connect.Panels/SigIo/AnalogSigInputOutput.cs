using System;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.SigIo
{
	public sealed class AnalogSigInputOutput : AbstractSigOutput
	{
		/// <summary>
		/// Raised when an analog value is sent by fusion.
		/// </summary>
		public event EventHandler<UShortEventArgs> OnOutput;

		/// <summary>
		/// Gets the sigtype we are interested in.
		/// </summary>
		protected override eSigType SigType { get { return eSigType.Analog; } }

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		public AnalogSigInputOutput(ISigDevice device)
			: base(device)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="number"></param>
		public AnalogSigInputOutput(ISigDevice device, uint number)
			: base(device, number)
		{
		}

		#endregion

		/// <summary>
		/// Sends the analog value to the device.
		/// </summary>
		/// <param name="value"></param>
		public void SendValue(ushort value)
		{
			Device.SendInputAnalog(Number, value);
		}

		/// <summary>
		/// Called when a subscribed output changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void DeviceOnOutput(SigCallbackManager sender, SigAdapterEventArgs args)
		{
			ushort value = args.Data.GetUShortValue();
			OnOutput.Raise(this, new UShortEventArgs(value));
		}
	}
}
