namespace ICD.Connect.Cameras.Visca
{
	public enum eViscaResponse
	{
		OK = 0,
		CLEAR = 1,
		ACK = 2,
		ADDRESS_SET = 3,
		MESSAGE_LENGTH_ERROR = 101,
		SYNTAX_ERROR = 102,
		COMMAND_BUFFER_FULL = 103,
		COMMAND_CANCELLED = 104,
		NO_SOCKET = 105,
		NOT_EXECUTABLE = 106,
		UNSPECIFIED = 107,
		NULL = 108,
		IMPROPER_FORMAT = 109,
	}

	public static class ViscaResponseExtensions
	{
		/// <summary>
		/// Returns true if the response code is an error.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool IsError(this eViscaResponse extends)
		{
			return (int)extends > 100;
		}
	}
}