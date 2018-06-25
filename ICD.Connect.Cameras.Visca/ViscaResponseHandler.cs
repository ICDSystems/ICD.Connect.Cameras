using System;
using ICD.Common.Utils;

namespace ICD.Connect.Cameras.Visca
{
	public static class ViscaResponseHandler
	{
		private const byte RESPONSE_ADDRESS_SET_HIGH = 0x3;
		private const byte RESPONSE_ACK_HIGH = 0x4;
		private const byte RESPONSE_OK_HIGH = 0x5;
		private const byte RESPONSE_ERR_HIGH = 0x6;
		private const byte RESPONSE_CLEAR_FIRST = 0x01;
		private const byte RESPONSE_CLEAR_SECOND = 0x00;

		public static eViscaResponse HandleResponse(string response)
		{
			if (String.IsNullOrEmpty(response))
				return eViscaResponse.NULL;

			byte[] responseBytes = StringUtils.ToBytes(response);

			//Responses are a minimum of 2 bytes
			if (responseBytes.Length < 2)
				return eViscaResponse.IMPROPER_FORMAT;

			if (responseBytes.Length > 3 && responseBytes[1] == RESPONSE_CLEAR_FIRST && responseBytes[2] == RESPONSE_CLEAR_SECOND)
				return eViscaResponse.CLEAR;

			// compare only the high nibble, low nibble changes based on command sockets
			byte responseHigh = (byte)(responseBytes[1] >> 4);

			if (responseHigh == RESPONSE_ADDRESS_SET_HIGH)
				return eViscaResponse.ADDRESS_SET;

			if (responseHigh == RESPONSE_ACK_HIGH)
				return eViscaResponse.ACK;

			if (responseHigh == RESPONSE_OK_HIGH)
				return eViscaResponse.OK;

			if (responseHigh == RESPONSE_ERR_HIGH)
			{
				switch (responseBytes[2])
				{
					case 0x01:
						return eViscaResponse.MESSAGE_LENGTH_ERROR;
					case 0x02:
						return eViscaResponse.SYNTAX_ERROR;
					case 0x03:
						return eViscaResponse.COMMAND_BUFFER_FULL;
					case 0x04:
						return eViscaResponse.COMMAND_CANCELLED;
					case 0x05:
						return eViscaResponse.NO_SOCKET;
					case 0x41:
						return eViscaResponse.NOT_EXECUTABLE;
					default:
						return eViscaResponse.UNSPECIFIED;
				}
			}

			return eViscaResponse.UNSPECIFIED;
		}

		public static bool ResponseIsError(eViscaResponse response)
		{
			return (int)response >= 100;
		}
	}
}