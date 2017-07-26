using System;
using ICD.Common.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels.EventArguments;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Panels.SigIo
{
	public sealed class DigitalSigInputOutput : AbstractSigOutput
	{
		/// <summary>
		/// Raised when a digital value is sent by fusion.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnOutput;

		/// <summary>
		/// Gets the sigtype we are interested in.
		/// </summary>
		protected override eSigType SigType { get { return eSigType.Digital; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		public DigitalSigInputOutput(ISigDevice device)
			: base(device)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="number"></param>
		public DigitalSigInputOutput(ISigDevice device, uint number)
			: base(device, number)
		{
		}

		/// <summary>
		/// Sends the serial value to the device.
		/// </summary>
		/// <param name="value"></param>
		public void SendValue(bool value)
		{
			Device.SendInputDigital(Number, value);
		}

		/// <summary>
		/// Called when a subscribed output changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void DeviceOnOutput(SigCallbackManager sender, SigAdapterEventArgs args)
		{
			bool value = args.Data.GetBoolValue();
			OnOutput.Raise(this, new BoolEventArgs(value));
		}
	}
}
