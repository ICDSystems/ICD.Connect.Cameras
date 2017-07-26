using System;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler.Requests
{
	public sealed class GetRegionsRequest : AbstractRequest<GetRegionsResult>
	{
		private const string ACTION = "GetRegions";

		/// <summary>
		/// Gets the name of the service method.
		/// </summary>
		protected override string SoapAction { get { return ACTION; } }

		/// <summary>
		/// Builds the resulting object from the xml response.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override GetRegionsResult ResultFromXml(string xml)
		{
			return GetRegionsResult.FromXml(xml);
		}

		/// <summary>
		/// Adds the parameters by calling addParam for each name/value pair.
		/// </summary>
		/// <param name="addParam"></param>
		protected override void AddSoapParams(Action<string, object> addParam)
		{
		}
	}
}
