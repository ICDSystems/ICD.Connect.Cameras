using System;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.SigIo
{
	public sealed class SerialSigInputOutput : AbstractSigOutput
	{
		/// <summary>
		/// Raised when a serial value is sent by fusion.
		/// </summary>
		public event EventHandler<StringEventArgs> OnOutput;

		/// <summary>
		/// Gets the sigtype we are interested in.
		/// </summary>
		protected override eSigType SigType { get { return eSigType.Serial; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		public SerialSigInputOutput(ISigDevice device)
			: base(device)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="number"></param>
		public SerialSigInputOutput(ISigDevice device, uint number)
			: base(device, number)
		{
		}

		/// <summary>
		/// Sends the serial value to the device.
		/// </summary>
		/// <param name="value"></param>
		public void SendValue(string value)
		{
			Device.SendInputSerial(Number, value);
		}

		/// <summary>
		/// Called when a subscribed output changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void DeviceOnOutput(SigCallbackManager sender, SigAdapterEventArgs args)
		{
			string value = args.Data.GetStringValue();
			OnOutput.Raise(this, new StringEventArgs(value));
		}
	}
}
