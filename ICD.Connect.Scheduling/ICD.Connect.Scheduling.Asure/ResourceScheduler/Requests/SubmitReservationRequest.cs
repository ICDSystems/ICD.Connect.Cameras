using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Requests
{
	public sealed class SubmitReservationRequest : AbstractRequest<SubmitReservationResult>
	{
		private const string ACTION = "SubmitReservation";

		private const string BODY_TEMPLATE = @"<x:Body>
        <ns:SubmitReservation>
            <ns:data>
                <ns:ReservationBaseData>
                    <ns:Description>{0}</ns:Description>
                    <ns:Notes>{1}</ns:Notes>
                    <ns:Resources>
                        {2}
                    </ns:Resources>
                </ns:ReservationBaseData>
                <ns:ScheduleData>
                    <ns:Start>{3}</ns:Start>
                    <ns:Duration>{4}</ns:Duration>
                </ns:ScheduleData>
            </ns:data>
        </ns:SubmitReservation>
    </x:Body>";

		private const string RESOURCE_TEMPLATE = @"<ns:ReservationResourceData>
                            <ns:Id>{0}</ns:Id>
                        </ns:ReservationResourceData>";

		private readonly string m_Description;
		private readonly string m_Notes;
		private readonly int[] m_ResourceIds;
		private readonly DateTime m_Start;
		private readonly DateTime m_End;

		/// <summary>
		/// Gets the name of the service method.
		/// </summary>
		protected override string SoapAction { get { return ACTION; } }

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="notes"></param>
		/// <param name="resourceIds"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public SubmitReservationRequest(string description, string notes, IEnumerable<int> resourceIds,
		                                DateTime start, DateTime end)
		{
			m_Description = description;
			m_Notes = notes;
			m_ResourceIds = resourceIds.ToArray();
			m_Start = start;
			m_End = end;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the body xml for the request.
		/// </summary>
		/// <returns></returns>
		protected override string GetBody()
		{
			string resources = GetResourcesXml(m_ResourceIds);
			string start = AsureUtils.DateTimeToString(m_Start);
			long duration = AsureUtils.GetDuration(m_Start, m_End);

			return string.Format(BODY_TEMPLATE, m_Description, m_Notes, resources, start, duration);
		}

		/// <summary>
		/// Builds the inner resources array xml.
		/// </summary>
		/// <param name="resources"></param>
		/// <returns></returns>
		private static string GetResourcesXml(IEnumerable<int> resources)
		{
			return GetIdsXml(RESOURCE_TEMPLATE, resources);
		}

		/// <summary>
		/// Builds inner array xml with the given string format.
		/// </summary>
		/// <param name="itemTemplate"></param>
		/// <param name="ids"></param>
		/// <returns></returns>
		private static string GetIdsXml(string itemTemplate, IEnumerable<int> ids)
		{
			string[] elements = ids.Select(i => string.Format(itemTemplate, i)).ToArray();
			return string.Join(string.Empty, elements);
		}

		/// <summary>
		/// Builds the resulting object from the xml response.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override SubmitReservationResult ResultFromXml(string xml)
		{
			return SubmitReservationResult.FromXml(xml);
		}

		#endregion
	}
}
