﻿using ICD.Common.Services.Logging;

namespace ICD.Common.Logging.Console.Loggers
{
	public interface ISystemLogger
	{
		void AddEntry(LogItem item);
	}
}
