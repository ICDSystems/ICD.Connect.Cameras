using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Requests;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;

namespace ICD.Connect.Scheduling.Asure.ResourceScheduler
{
	public static class ResourceSchedulerService
	{
		#region Methods

		/// <summary>
		/// Checks in to the given reservation.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="reservationId"></param>
		/// <returns></returns>
		[PublicAPI]
		public static CheckInResult CheckIn(IWebPort port, string username, string password, int reservationId)
		{
			return new CheckInRequest(reservationId).Dispatch(port, username, password);
		}

		[PublicAPI]
		public static CheckInNowResult CheckInNow(IWebPort port, string username, string password, int reservationId)
		{
			return new CheckInNowRequest(reservationId).Dispatch(port, username, password);
		}

		/// <summary>
		/// Checks out of the given reservation.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="reservationId"></param>
		/// <returns></returns>
		[PublicAPI]
		public static CheckOutResult CheckOut(IWebPort port, string username, string password, int reservationId)
		{
			return new CheckOutRequest(reservationId).Dispatch(port, username, password);
		}

		/// <summary>
		/// Deletes the reservation with the given id.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="reservationId"></param>
		/// <returns></returns>
		[PublicAPI]
		public static DeleteReservationResult DeleteReservation(IWebPort port, string username, string password,
		                                                        int reservationId)
		{
			return new DeleteReservationRequest(reservationId).Dispatch(port, username, password);
		}

		/// <summary>
		/// Gets the regions.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		[PublicAPI]
		public static GetRegionsResult GetRegions(IWebPort port, string username, string password)
		{
			return new GetRegionsRequest().Dispatch(port, username, password);
		}

		/// <summary>
		/// Gets the locations.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		[PublicAPI]
		public static GetLocationsResult GetLocations(IWebPort port, string username, string password)
		{
			return new GetLocationsRequest().Dispatch(port, username, password);
		}

		/// <summary>
		/// Gets the locations for the given region.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="regionId"></param>
		/// <returns></returns>
		[PublicAPI]
		public static GetLocationsResult GetLocations(IWebPort port, string username, string password, int regionId)
		{
			return new GetLocationsRequest(regionId).Dispatch(port, username, password);
		}

		/// <summary>
		/// Gets all regions and their child locations.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		[PublicAPI]
		public static GetAllRegionsAndLocationsResult GetAllRegionsAndLocations(IWebPort port, string username,
		                                                                        string password)
		{
			return new GetAllRegionsAndLocationsRequest().Dispatch(port, username, password);
		}

		/// <summary>
		/// Gets the reservation with the given id.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[PublicAPI]
		public static GetReservationResult GetReservation(IWebPort port, string username, string password, int id)
		{
			return new GetReservationRequest(id).Dispatch(port, username, password);
		}

		/// <summary>
		/// Gets all of the reservations between the start and end dates.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		[PublicAPI]
		public static GetReservationsResult GetReservations(IWebPort port, string username, string password, DateTime start,
		                                                    DateTime end)
		{
			return new GetReservationsRequest(start, end).Dispatch(port, username, password);
		}

		/// <summary>
		/// Gets all of the reservations between the start and end date with the given attendee.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="attendeeId"></param>
		/// <returns></returns>
		[PublicAPI]
		public static GetReservationsByAttendeeResult GetReservationsByAttendee(IWebPort port, string username,
		                                                                        string password, DateTime start,
		                                                                        DateTime end, int attendeeId)
		{
			return new GetReservationsByAttendeeRequest(start, end, attendeeId).Dispatch(port, username, password);
		}

		/// <summary>
		/// Gets all of the reservations between the start and end date with the given location.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="locationId"></param>
		/// <returns></returns>
		[PublicAPI]
		public static GetReservationsByLocationResult GetReservationsByLocation(IWebPort port, string username,
		                                                                        string password, DateTime start,
		                                                                        DateTime end, int locationId)
		{
			return new GetReservationsByLocationRequest(start, end, locationId).Dispatch(port, username, password);
		}

		/// <summary>
		/// Gets all of the reservations between the start and end date with the given resource.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="resourceId"></param>
		/// <returns></returns>
		[PublicAPI]
		public static GetReservationsByResourceResult GetReservationsByResource(IWebPort port, string username,
		                                                                        string password, DateTime start,
		                                                                        DateTime end, int resourceId)
		{
			return new GetReservationsByResourceRequest(start, end, resourceId).Dispatch(port, username, password);
		}

		/// <summary>
		/// Creates a new reservation.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="description"></param>
		/// <param name="notes"></param>
		/// <param name="resourceIds"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		[PublicAPI]
		public static SubmitReservationResult SubmitReservation(IWebPort port, string username, string password,
		                                                        string description, string notes,
		                                                        IEnumerable<int> resourceIds,
		                                                        DateTime start, DateTime end)
		{
			SubmitReservationRequest request = new SubmitReservationRequest(description, notes, resourceIds, start, end);
			return request.Dispatch(port, username, password);
		}

		#endregion
	}
}
