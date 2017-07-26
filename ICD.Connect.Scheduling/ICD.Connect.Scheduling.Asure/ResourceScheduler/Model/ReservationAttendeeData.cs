using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Model
{
	public sealed class ReservationAttendeeData : AbstractData
	{
		public const string ELEMENT = "ReservationAttendeeData";

		#region Properties

		[PublicAPI]
		public int Id { get; private set; }

		[PublicAPI]
		public string FullName { get; private set; }

		[PublicAPI]
		public string EmailAddress { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a ReservationAttendeeData instance from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static ReservationAttendeeData FromXml(string xml)
		{
			return new ReservationAttendeeData
			{
				Id = XmlUtils.ReadChildElementContentAsInt(xml, "Id"),
				FullName = XmlUtils.ReadChildElementContentAsString(xml, "FullName"),
				EmailAddress = XmlUtils.ReadChildElementContentAsString(xml, "EmailAddress")
			};
		}

		#endregion
	}
}
