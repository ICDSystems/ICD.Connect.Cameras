using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Model
{
	public sealed class ReservationData : AbstractData
	{
		public const string ELEMENT = "ReservationData";

		[PublicAPI]
		public ReservationBaseData ReservationBaseData { get; private set; }

		[PublicAPI]
		public ScheduleData ScheduleData { get; private set; }

		[PublicAPI]
		public int CheckedInBy { get; private set; }

		/// <summary>
		/// Convenience for determining if the reservation is currently checked in.
		/// </summary>
		[PublicAPI]
		public bool CheckedIn { get { return CheckedInBy != 0; } }

		[PublicAPI]
		public bool RequiresCheckInCheckOut { get; private set; }

		/// <summary>
		/// Instantiates a ReservationData instance from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static ReservationData FromXml(string xml)
		{
			string reservationBaseDataXml = XmlUtils.GetChildElementAsString(xml, ReservationBaseData.ELEMENT);
			string scheduleDataXml = XmlUtils.GetChildElementAsString(xml, ScheduleData.ELEMENT);

			return new ReservationData
			{
				ReservationBaseData = ReservationBaseData.FromXml(reservationBaseDataXml),
				ScheduleData = ScheduleData.FromXml(scheduleDataXml),
				CheckedInBy = XmlUtils.TryReadChildElementContentAsInt(xml, "CheckedInBy") ?? 0,
				RequiresCheckInCheckOut = XmlUtils.TryReadChildElementContentAsBoolean(xml, "RequiresCheckInCheckOut") ?? false
			};
		}
	}
}
