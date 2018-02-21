using System;
using ICD.Common.Utils;

namespace ICD.Connect.Cameras.Visca
{
	public static class ViscaResponseHandler
	{
		private const byte CLEAR_RESPONSE_FIRST = 0x88;
		private const byte CLEAR_RESPONSE_SECOND = 0x30;
		private const byte RESPONSE_OK = 0x50;
		private const byte RESPONSE_ERR = 0x60;


		public static eViscaResponse HandleResponse(string response)
		{
			if (String.IsNullOrEmpty(response))
				return eViscaResponse.NULL;

			byte[] responseBytes = StringUtils.ToBytes(response);

			//Responses are a minimum of 2 bytes
			if (responseBytes.Length < 2)
				return eViscaResponse.IMPROPER_FORMAT;

			if (responseBytes[0] == CLEAR_RESPONSE_FIRST && responseBytes[1] == CLEAR_RESPONSE_SECOND)
				return eViscaResponse.OK;

			if (responseBytes[1] == RESPONSE_OK)
				return eViscaResponse.OK;

			if (responseBytes[1] == RESPONSE_ERR)
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
	}
}