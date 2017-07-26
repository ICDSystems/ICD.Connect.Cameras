using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Commands;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Scheduling.Asure.ResourceScheduler;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Model;
using ICD.Connect.Scheduling.Asure.ResourceScheduler.Results;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Scheduling.Asure
{
	/// <summary>
	/// Provides a means of communicate with the Asure SOAP service for reservation management.
	/// </summary>
	public sealed class AsureDevice : AbstractDevice<AsureDeviceSettings>
	{
		// Update every 5 minutes by default.
		private const long DEFAULT_REFRESH_INTERVAL = 5 * 60 * 1000;

		/// <summary>
		/// Raised when the internal reservation cache has been updated with the latest information.
		/// </summary>
		public event EventHandler OnCacheUpdated;

		private IWebPort m_Port;

		private readonly SafeTimer m_UpdateTimer;

		private bool m_Cached;
		private readonly Dictionary<int, ReservationData> m_Cache;
		private readonly SafeCriticalSection m_CacheSection;

		#region Properties

		/// <summary>
		/// Gets/sets ID for the resource to be used when making bookings.
		/// </summary>
		[PublicAPI]
		public int ResourceId { get; set; }

		/// <summary>
		/// Gets/sets how often to refresh the cached resources in milliseconds.
		/// </summary>
		[PublicAPI]
		public long UpdateInterval { get; set; }

		/// <summary>
		/// Gets/sets the username for communication with the service.
		/// </summary>
		[PublicAPI]
		public string Username { get; set; }

		/// <summary>
		/// Gets/sets the password for communication with the service.
		/// </summary>
		[PublicAPI]
		public string Password { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public AsureDevice()
		{
			m_Cache = new Dictionary<int, ReservationData>();
			m_CacheSection = new SafeCriticalSection();

			m_UpdateTimer = new SafeTimer(RefreshCacheInternal, DEFAULT_REFRESH_INTERVAL, DEFAULT_REFRESH_INTERVAL);
			UpdateInterval = DEFAULT_REFRESH_INTERVAL;
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnCacheUpdated = null;

			m_UpdateTimer.Dispose();

			base.DisposeFinal(disposing);

			SetPort(null);
		}

		/// <summary>
		/// Sets the port for communication with the service.
		/// </summary>
		/// <param name="port"></param>
		[PublicAPI]
		public void SetPort(IWebPort port)
		{
			if (port == m_Port)
				return;

			Unsubscribe(m_Port);

			if (port != null)
				port.Accept = "application/xml";

			m_Port = port;
			Subscribe(m_Port);

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Gets the cached reservations.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public IEnumerable<ReservationData> GetReservations()
		{
			if (!m_Cached)
				RefreshCacheInternal();

			m_CacheSection.Enter();

			try
			{
				return m_Cache.Values
				              .Where(r => r.ScheduleData.End > IcdEnvironment.GetLocalTime())
				              .OrderBy(r => r.ScheduleData.Start)
				              .ToArray();
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Returns true if a reservation is currently scheduled.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public bool GetReservationInProgress()
		{
			return GetCurrentReservation() != null;
		}

		/// <summary>
		/// Returns true if a reservation is in progress but less than half way elapsed.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public bool CanCheckIn()
		{
			// Can't check in if already checked in.
			if (GetCheckedInState())
				return false;

			ReservationData reservation = GetCurrentReservation();
			if (reservation == null)
				return false;

			if (!reservation.RequiresCheckInCheckOut)
				return false;

			if (reservation.ScheduleData.Start == null || reservation.ScheduleData.End == null)
				return false;

			// Right now we only want to check in to a reservation if less than half time has elapsed.
			// This probably needs to move into metlife specific logic eventually.
			double delta = MathUtils.MapRange((DateTime)reservation.ScheduleData.Start,
			                                  (DateTime)reservation.ScheduleData.End,
			                                  IcdEnvironment.GetLocalTime());
			return delta >= 0.0f && delta <= 0.5f;
		}

		[PublicAPI]
		public bool CanCheckOut()
		{
			// Can't check out if already checked out
			if (!GetCheckedInState())
				return false;

			ReservationData reservation = GetCurrentReservation();
			return reservation != null && reservation.RequiresCheckInCheckOut;
		}

		/// <summary>
		/// Returns true if the current reservation has been checked into.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		public bool GetCheckedInState()
		{
			ReservationData reservation = GetCurrentReservation();
			return reservation != null && reservation.CheckedIn;
		}

		/// <summary>
		/// Gets the current reservation in progress.
		/// </summary>
		/// <returns></returns>
		[PublicAPI, CanBeNull]
		public ReservationData GetCurrentReservation()
		{
			return GetReservations().Where(IsReservationCurrent).FirstOrDefault();
		}

		/// <summary>
		/// Gets the next scheduled reservation.
		/// </summary>
		/// <returns></returns>
		[PublicAPI, CanBeNull]
		public ReservationData GetNextReservation()
		{
			ReservationData current = GetCurrentReservation();
			if (current == null)
				return GetReservations().FirstOrDefault();

			return GetReservations().SkipWhile(r => r.ReservationBaseData.Id != current.ReservationBaseData.Id)
			                        .Skip(1)
			                        .FirstOrDefault();
		}

		/// <summary>
		/// Returns true if the reservation is currently taking place.
		/// </summary>
		/// <param name="reservation"></param>
		/// <returns></returns>
		[PublicAPI]
		public static bool IsReservationCurrent(ReservationData reservation)
		{
			DateTime now = IcdEnvironment.GetLocalTime();
			return reservation.ScheduleData.Start <= now && reservation.ScheduleData.End > now;
		}

		/// <summary>
		/// Check in to the reservation with the given id.
		/// </summary>
		/// <param name="reservationId"></param>
		/// <exception cref="InvalidOperationException"></exception>
		[PublicAPI]
		public void CheckIn(int reservationId)
		{
			CheckInResult result = ResourceSchedulerService.CheckIn(m_Port, Username, Password, reservationId);
			if (result.IsValid)
				InsertReservation(result.ReservationData);
			else
				throw new InvalidOperationException(result.AllChildBrokenBusinessRules.First().Description);
		}

		/// <summary>
		/// Check in to the reservation with the given id.
		/// </summary>
		/// <param name="reservationId"></param>
		/// <exception cref="InvalidOperationException"></exception>
		[PublicAPI]
		public void CheckOut(int reservationId)
		{
			CheckOutResult result = ResourceSchedulerService.CheckOut(m_Port, Username, Password, reservationId);
			if (result.IsValid)
				InsertReservation(result.ReservationData);
			else
				throw new InvalidOperationException(result.AllChildBrokenBusinessRules.First().Description);
		}

		/// <summary>
		/// Submit a new reservation for the given duration.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="notes"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <exception cref="InvalidOperationException"></exception>
		[PublicAPI]
		public void SubmitReservation(string description, string notes, DateTime start, DateTime end)
		{
			SubmitReservationResult result =
				ResourceSchedulerService.SubmitReservation(m_Port, Username, Password, description, notes,
				                                           new[] {ResourceId}, start, end);

			if (result.IsValid)
				InsertReservation(result.ReservationData);
			else
				throw new InvalidOperationException(result.AllChildBrokenBusinessRules.First().Description);
		}

		/// <summary>
		/// Refreshes the cache and raises OnCacheUpdated.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		[PublicAPI]
		public void RefreshCache()
		{
			DateTime start = DateTime.Today;
			DateTime end = DateTime.Today.AddDays(1);

			GetReservationsByResourceResult result =
				ResourceSchedulerService.GetReservationsByResource(m_Port, Username, Password, start, end, ResourceId);

			if (!result.IsValid)
				throw new InvalidOperationException(result.AllChildBrokenBusinessRules.First().Description);

			ReservationData[] data = result
				.ReservationData
				.Where(r => r.ScheduleData.End > IcdEnvironment.GetLocalTime())
				.ToArray();

			m_CacheSection.Enter();
			try
			{
				m_Cache.Clear();
				m_Cache.AddRange(data, r => r.ReservationBaseData.Id);
			}
			finally
			{
				m_CacheSection.Leave();
			}

			m_Cached = true;
			OnCacheUpdated.Raise(this);
		}

		/// <summary>
		/// Refreshes the cache asynchronously and raises OnCacheUpdated.
		/// </summary>
		[PublicAPI]
		public void RefreshCacheAsync()
		{
			CrestronUtils.SafeInvoke(RefreshCache);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Refreshes the cache. Catches any exceptions and logs them.
		/// </summary>
		private void RefreshCacheInternal()
		{
			try
			{
				RefreshCache();
			}
			catch (Exception e)
			{
				Logger.AddEntry(eSeverity.Error, "Failed to refresh Asure cache - {0}", e.Message);
			}
		}

		/// <summary>
		/// Inserts a reservation into the cache, replacing an existing reservation with the same id.
		/// 
		/// </summary>
		/// <param name="reservation"></param>
		private void InsertReservation(ReservationData reservation)
		{
			m_CacheSection.Enter();
			try
			{
				m_Cache[reservation.ReservationBaseData.Id] = reservation;
			}
			finally
			{
				m_CacheSection.Leave();
			}

			OnCacheUpdated.Raise(this);
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Port != null && m_Port.IsOnline;
		}

		#endregion

		#region Port Callbacks

		/// <summary>
		/// Subscribe to the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(IWebPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(IWebPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged -= PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Called when the port online state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void PortOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Username = null;
			Password = null;
			SetPort(null);
			ResourceId = 0;
			UpdateInterval = DEFAULT_REFRESH_INTERVAL;
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(AsureDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Username = Username;
			settings.Password = Password;
			settings.ResourceId = ResourceId;
			settings.UpdateInterval = UpdateInterval;
			settings.Port = m_Port == null ? (int?)null : m_Port.Id;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(AsureDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			Username = settings.Username;
			Password = settings.Password;

			IWebPort port = null;

			if (settings.Port != null)
			{
				port = factory.GetPortById((int)settings.Port) as IWebPort;
				if (port == null)
					Logger.AddEntry(eSeverity.Error, "No web port with id {0}", settings.Port);
			}

			SetPort(port);

			ResourceId = settings.ResourceId;
			UpdateInterval = settings.UpdateInterval ?? DEFAULT_REFRESH_INTERVAL;
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand item in GetBaseConsoleCommands())
				yield return item;

			yield return new ConsoleCommand("RefreshCache", "Updates the cache from the Asure web service", () => RefreshCacheConsole());
			yield return
				new ConsoleCommand("PrintReservations", "Outputs the cached reservations to the console", () => PrintReservations());
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		/// <summary>
		/// Refreshes the cache and prints information to the console.
		/// </summary>
		private string RefreshCacheConsole()
		{
			try
			{
				RefreshCache();
			}
			catch (Exception e)
			{
				return string.Format("Failed to refresh Asure cache - {0}", e.Message);
			}

			return PrintReservations();
		}

		/// <summary>
		/// Prints the cached reservations to the console.
		/// </summary>
		private string PrintReservations()
		{
			TableBuilder builder = new TableBuilder("ID", "Description", "Attendees", "Resources", "Start", "End", "Checked-In");

			foreach (ReservationData reservation in GetReservations())
			{
				builder.AddRow(reservation.ReservationBaseData.Id,
				               reservation.ReservationBaseData.Description,
				               StringUtils.ArrayFormat(reservation.ReservationBaseData.ReservationAttendees),
				               StringUtils.ArrayFormat(reservation.ReservationBaseData.ReservationResources),
				               reservation.ScheduleData.Start,
				               reservation.ScheduleData.End,
				               reservation.CheckedIn);
			}

			return builder.ToString();
		}

		#endregion
	}
}
