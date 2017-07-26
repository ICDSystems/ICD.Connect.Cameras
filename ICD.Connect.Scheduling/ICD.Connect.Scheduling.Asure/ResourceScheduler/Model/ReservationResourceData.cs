using ICD.Common.Properties;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Model
{
	public sealed class ReservationResourceData : AbstractData
	{
		public const string ELEMENT = "ReservationResourceData";

		[PublicAPI]
		public int Id { get; private set; }

		[PublicAPI]
		public string Description { get; private set; }

		/// <summary>
		/// Instantiates the ReservationResourceData from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static ReservationResourceData FromXml(string xml)
		{
			return new ReservationResourceData
			{
				Id = XmlUtils.ReadChildElementContentAsInt(xml, "Id"),
				Description = XmlUtils.ReadChildElementContentAsString(xml, "Description")
			};
		}
	}
}
