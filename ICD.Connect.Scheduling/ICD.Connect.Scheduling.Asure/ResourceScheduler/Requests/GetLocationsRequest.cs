using System;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Requests
{
	public sealed class GetLocationsRequest : AbstractRequest<GetLocationsResult>
	{
		private const string ACTION = "GetLocations";

		private readonly int? m_RegionId;

		/// <summary>
		/// Gets the name of the service method.
		/// </summary>
		protected override string SoapAction { get { return ACTION; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public GetLocationsRequest()
			: this(null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="regionId"></param>
		public GetLocationsRequest(int? regionId)
		{
			m_RegionId = regionId;
		}

		/// <summary>
		/// Builds the resulting object from the xml response.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override GetLocationsResult ResultFromXml(string xml)
		{
			return GetLocationsResult.FromXml(xml);
		}

		/// <summary>
		/// Adds the parameters by calling addParam for each name/value pair.
		/// </summary>
		/// <param name="addParam"></param>
		protected override void AddSoapParams(Action<string, object> addParam)
		{
			if (m_RegionId != null)
				addParam("RegionId", (int)m_RegionId);
		}
	}
}
