using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Model
{
	public sealed class ScheduleData : AbstractData
	{
		public const string ELEMENT = "ScheduleData";

		private DateTime? m_End;

		[PublicAPI]
		public DateTime? Start { get; private set; }

		[PublicAPI]
		public DateTime? End
		{
			get
			{
				if (m_End != null)
					return m_End;

				if (Start == null)
					return null;

				return AsureUtils.GetEndDateTime((DateTime)Start, Duration);
			}
			private set { m_End = value; }
		}

		[PublicAPI]
		public DateTime? StartAdjusted { get; private set; }

		[PublicAPI]
		public DateTime? EndAdjusted { get; private set; }

		[PublicAPI]
		public long Duration { get; private set; }

		[PublicAPI]
		public string TimeZoneId { get; private set; }

		/// <summary>
		/// Instantiates a ScheduleData from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static ScheduleData FromXml(string xml)
		{
			string start = XmlUtils.TryReadChildElementContentAsString(xml, "Start");
			string end = XmlUtils.TryReadChildElementContentAsString(xml, "End");
			string startAdjusted = XmlUtils.TryReadChildElementContentAsString(xml, "StartAdjusted");
			string endAdjusted = XmlUtils.TryReadChildElementContentAsString(xml, "EndAdjusted");
			long duration = XmlUtils.ReadChildElementContentAsLong(xml, "Duration");
			string timeZoneId = XmlUtils.TryReadChildElementContentAsString(xml, "TimeZoneId");

			ScheduleData output = new ScheduleData
			{
				Duration = duration,
				TimeZoneId = timeZoneId
			};

			if (!string.IsNullOrEmpty(start))
				output.Start = AsureUtils.DateTimeFromString(start);

			if (!string.IsNullOrEmpty(end))
				output.End = AsureUtils.DateTimeFromString(end);

			if (!string.IsNullOrEmpty(startAdjusted))
				output.StartAdjusted = AsureUtils.DateTimeFromString(startAdjusted);

			if (!string.IsNullOrEmpty(endAdjusted))
				output.EndAdjusted = AsureUtils.DateTimeFromString(endAdjusted);

			return output;
		}
	}
}
