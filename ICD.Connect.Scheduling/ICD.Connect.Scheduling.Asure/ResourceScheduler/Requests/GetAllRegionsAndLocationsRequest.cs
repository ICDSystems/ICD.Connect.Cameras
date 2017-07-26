using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Requests
{
	public sealed class GetAllRegionsAndLocationsRequest : AbstractRequest<GetAllRegionsAndLocationsResult>
	{
		private const string ACTION = "GetAllRegionsAndLocations";

		/// <summary>
		/// Gets the name of the service method.
		/// </summary>
		protected override string SoapAction { get { return ACTION; } }

		/// <summary>
		/// Builds the resulting object from the xml response.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override GetAllRegionsAndLocationsResult ResultFromXml(string xml)
		{
			return GetAllRegionsAndLocationsResult.FromXml(xml);
		}
	}
}
