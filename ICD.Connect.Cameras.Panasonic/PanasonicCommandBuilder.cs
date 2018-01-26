using System;
using System.Text;
using ICD.Common.Properties;

namespace ICD.Connect.Cameras.Panasonic
{
	public static class PanasonicCommandBuilder
	{
		#region String Constants
		private const string PTS = "PTS";
		private const string ZOOM = "Z";
		private const string POWER = "O";
		#endregion

		#region Default Speeds
		private const int DEFAULT_SPEED = 24;
		private const int STOP_SPEED = 50;
		#endregion

		#region Public Commands
		/// <summary>
		/// Gets the Power On Command URL
		/// </summary>
		[PublicAPI]
		public static string GetPowerOnCommand()
		{
			return GetCommandUrl(POWER, 1);
		}

		/// <summary>
		/// Gets the Power Off Command URL
		/// </summary>
		[PublicAPI]
		public static string GetPowerOffCommand()
		{
			return GetCommandUrl(POWER, 0);
		}

		/// <summary>
		/// Gets the Power Query Command URL
		/// </summary>
		[PublicAPI]
		public static string GetPowerQueryCommand()
		{
			return GetCommandUrl(POWER);
		}

		/// <summary>
		/// Gets the Pan/Tilt Command URL, using the default speed
		/// </summary>
		/// <param name="action">The Pan/Tilt action desired.</param>
		[PublicAPI]
		public static string GetPanTiltCommand(eCameraPanTiltAction action)
		{
			return GetPanTiltCommand(action, DEFAULT_SPEED);
		}

		/// <summary>
		/// Gets the Pan/Tilt Command URL, using the speed provided.
		/// </summary>
		/// <param name="action">The Pan/Tilt action desired.</param>
		/// <param name="speed">The desired speed, where 0 is still and 50 is fastest possible</param>
		[PublicAPI]
		public static string GetPanTiltCommand(eCameraPanTiltAction action, int speed)
		{
			string speedString = GetSpeedStringBasedOnDirection(action, speed);
			switch (action)
			{
				case eCameraPanTiltAction.Left:
				case eCameraPanTiltAction.Right:
					return GetCommandUrl(PTS, speedString, STOP_SPEED);
				case eCameraPanTiltAction.Up:
				case eCameraPanTiltAction.Down:
					return GetCommandUrl(PTS, STOP_SPEED, speedString);
				case eCameraPanTiltAction.Stop:
					return GetPanTiltStopCommand();
				default:
					throw new ArgumentOutOfRangeException("action");
			}
		}

		/// <summary>
		/// Gets the Zoom Command URL, using the default speed
		/// </summary>
		/// <param name="action">The Zoom action desired.</param>
		[PublicAPI]
		public static string GetZoomCommand(eCameraZoomAction action)
		{
			return GetZoomCommand(action, DEFAULT_SPEED);
		}

		/// <summary>
		/// Gets the Zoom Command URL, using the speed provided.
		/// </summary>
		/// <param name="action">The Zoom action desired.</param>
		/// <param name="speed">The desired speed, where 0 is still and 50 is fastest possible</param>
		[PublicAPI]
		public static string GetZoomCommand(eCameraZoomAction action, int speed)
		{
			string speedString = GetSpeedStringBasedOnDirection(action, speed);
			switch (action)
			{
				case eCameraZoomAction.ZoomIn:
				case eCameraZoomAction.ZoomOut:
					return GetCommandUrl(ZOOM, speedString);
				case eCameraZoomAction.Stop:
					return GetStopZoomCommand();
				default:
					throw new ArgumentOutOfRangeException("action");
			}
		}
		#endregion

		#region Builders
		private static string GetPanTiltStopCommand()
		{
			return GetCommandUrl(PTS, STOP_SPEED, STOP_SPEED);
		}

		private static string GetStopZoomCommand()
		{
			return GetCommandUrl(ZOOM, STOP_SPEED);
		}

		private static string GetCommandUrl(string command)
		{
			return string.Format("cgi-bin/aw_ptz?cmd=%23{0}&res=1", command);
		}

		private static string GetCommandUrl(string command, params object[] parameters)
		{
			StringBuilder builder = new StringBuilder(command);
			foreach (object parameter in parameters)
				builder.Append(parameter);

			return GetCommandUrl(builder.ToString());
		}

		private static string GetSpeedStringBasedOnDirection(eCameraPanTiltAction action, int speed)
		{
			int returnSpeed;

			switch (action)
			{
				case eCameraPanTiltAction.Down:
					returnSpeed = STOP_SPEED - speed;
					break;
				case eCameraPanTiltAction.Up:
					returnSpeed = STOP_SPEED + speed;
					break;
				case eCameraPanTiltAction.Left:
					returnSpeed = STOP_SPEED - speed;
					break;
				case eCameraPanTiltAction.Right:
					returnSpeed = STOP_SPEED + speed;
					break;
				case eCameraPanTiltAction.Stop:
					returnSpeed = STOP_SPEED;
					break;
				default:
					throw new ArgumentOutOfRangeException("action");
			}

			return string.Format("{0:00}", returnSpeed);
		}

		private static string GetSpeedStringBasedOnDirection(eCameraZoomAction action, int speed)
		{
			int returnSpeed;

			switch (action)
			{
				case eCameraZoomAction.ZoomIn:
					returnSpeed = STOP_SPEED + speed;
					break;
				case eCameraZoomAction.ZoomOut:
					returnSpeed = STOP_SPEED - speed;
					break;
				case eCameraZoomAction.Stop:
					returnSpeed = STOP_SPEED;
					break;
				default:
					throw new ArgumentOutOfRangeException("action");
			}

			return string.Format("{0:00}", returnSpeed);
		}
		#endregion
	}
}
				