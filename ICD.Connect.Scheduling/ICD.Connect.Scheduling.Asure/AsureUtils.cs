using System;
using System.Globalization;

namespace ICD.Connect.Scheduling.Asure
{
	/// <summary>
	/// Util methods for working with the Asure services.
	/// </summary>
	public static class AsureUtils
	{
		// 2016-09-05 08:00:00
		private const string XML_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

		/// <summary>
		/// Parses a DateTime from an xml response string.
		/// </summary>
		/// <param name="dateTimeString"></param>
		/// <returns></returns>
		/// <exception cref="FormatException"></exception>
		public static DateTime DateTimeFromString(string dateTimeString)
		{
			return DateTime.ParseExact(dateTimeString, XML_DATETIME_FORMAT, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parses a DateTime to an xml request string.
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static string DateTimeToString(DateTime dateTime)
		{
			return dateTime.ToString(XML_DATETIME_FORMAT);
		}

		/// <summary>
		/// Returns the delta between start and end as a duration in ticks.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static long GetDuration(DateTime start, DateTime end)
		{
			TimeSpan span = end - start;
			return span.Ticks;
		}

		/// <summary>
		/// Returns the datetime after adding the duration (ticks) to the start date.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		public static DateTime GetEndDateTime(DateTime start, long duration)
		{
			return start + new TimeSpan(duration);
		}
	}
}
