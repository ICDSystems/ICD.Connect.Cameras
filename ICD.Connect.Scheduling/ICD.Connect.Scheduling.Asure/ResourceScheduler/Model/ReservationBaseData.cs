using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Model
{
	public sealed class ReservationBaseData : AbstractData
	{
		public const string ELEMENT = "ReservationBaseData";

		#region Properties

		[PublicAPI]
		public int Id { get; private set; }

		[PublicAPI]
		public string Description { get; private set; }

		[PublicAPI]
		public string Notes { get; private set; }

		[PublicAPI]
		public ReservationAttendeeData[] ReservationAttendees { get; private set; }

		[PublicAPI]
		public ReservationResourceData[] ReservationResources { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Instantiates a ReservationBaseData instance from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static ReservationBaseData FromXml(string xml)
		{
			ReservationBaseData output = new ReservationBaseData
			{
				Id = XmlUtils.ReadChildElementContentAsInt(xml, "Id"),
				Description = XmlUtils.ReadChildElementContentAsString(xml, "Description"),
				Notes = XmlUtils.ReadChildElementContentAsString(xml, "Notes")
			};

			string attendeesXml = XmlUtils.GetChildElementAsString(xml, "Attendees");
			output.ReservationAttendees = XmlUtils.GetChildElementsAsString(attendeesXml, ReservationAttendeeData.ELEMENT)
			                                      .Select(x => ReservationAttendeeData.FromXml(x))
			                                      .ToArray();

			string resourcesXml = XmlUtils.GetChildElementAsString(xml, "Resources");
			output.ReservationResources = XmlUtils.GetChildElementsAsString(resourcesXml, ReservationResourceData.ELEMENT)
			                                      .Select(x => ReservationResourceData.FromXml(x))
			                                      .ToArray();

			return output;
		}

		#endregion
	}
}
