using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Protocol.Data;
using ICD.Connect.Protocol.EventArguments;

namespace ICD.Connect.Cameras.Visca.Aver
{
	public sealed class AverPtz3ViscaCameraDevice : AbstractViscaCameraDevice<AverPtz3ViscaCameraDeviceSettings>
	{
		private uint m_Sequence;

		/// <summary>
		/// Queues the command to be sent to the device.
		/// </summary>
		/// <param name="command"></param>
		protected override void SendCommand(ViscaCommand command)
		{
			string data = command.Serialize();

			// Prepend the header
			byte[] header =
			{
				0x01,
				0x00,
				0x00,
				(byte)data.Length
			};

			// Sequence
			uint sequence;
			unchecked
			{
				sequence = m_Sequence++;
			}
			byte[] sequenceBytes = BitConverter.GetBytes(sequence).Reverse().ToArray();

			data = StringUtils.ToString(header) + StringUtils.ToString(sequenceBytes) + data;
			SerialQueue.Enqueue(new SerialData(data));
		}

		/// <summary>
		/// Called when a complete response is received from the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void SerialQueueOnSerialResponse(object sender, SerialResponseEventArgs args)
		{
			if (args.Data == null)
				return;

			// Strip the header
			string data = args.Response.Substring(8);
			eViscaResponse code = ViscaResponseUtils.ToResponse(data);

			// Convert the sent SerialData back to a ViscaCommand
			ViscaCommand command = new ViscaCommand(StringUtils.ToBytes(args.Data.Serialize()));

			if (code.IsError())
				HandleError(command, code);
			else
				HandleSuccess(command, args.Response, code);
		}
	}
}
