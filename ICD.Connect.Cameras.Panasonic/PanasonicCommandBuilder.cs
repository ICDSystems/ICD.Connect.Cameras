using System;
using System.Text;
using ICD.Connect.Conferencing.Cameras;

namespace ICD.Connect.Cameras.Panasonic
{
	public static class PanasonicCommandBuilder
	{
		private const string PTS = "PTS";
		private const string ZOOM = "Z";
		private const string POWER = "O";

		private const int DEFAULT_SPEED = 24;
		private const int STOP_SPEED = 50;

		public static string PowerOn()
		{
			return GetCommandUrl(POWER, 1);
		}

		public static string PowerOff()
		{
			return GetCommandUrl(POWER, 0);
		}

		public static string PowerQuery()
		{
			return GetCommandUrl(POWER);
		}

		public static string Stop()
		{
			return GetCommandUrl(PTS, STOP_SPEED, STOP_SPEED);
		}

		public static string StopZoom()
		{
			return GetCommandUrl(ZOOM, STOP_SPEED);
		}

		public static string Move(eCameraAction action)
		{
			return Move(action, DEFAULT_SPEED);
		}

		public static string Move(eCameraAction action, int speed)
		{
			string speedString = GetSpeedStringBasedOnDirection(action, speed);

			switch (action)
			{
				case eCameraAction.Down:
				case eCameraAction.Up:
					return GetCommandUrl(PTS, STOP_SPEED, speedString);

				case eCameraAction.Left:
				case eCameraAction.Right:
					return GetCommandUrl(PTS, speedString, STOP_SPEED);

				case eCameraAction.ZoomIn:
				case eCameraAction.ZoomOut:
					return GetCommandUrl(ZOOM, speedString);

				default:
					throw new ArgumentOutOfRangeException("action");
			}
		}

		private static string GetCommandUrl(string command, params object[] parameters)
		{
			StringBuilder builder = new StringBuilder(command);
			foreach (object parameter in parameters)
				builder.Append(parameter);

			return GetCommandUrl(builder.ToString());
		}

		private static string GetCommandUrl(string command)
		{
			return string.Format("cgi-bin/aw_ptz?cmd=%23{0}&res=1", command);
		}

		private static string GetSpeedStringBasedOnDirection(eCameraAction action, int speed)
		{
			int returnSpeed;

			switch (action)
			{
				case eCameraAction.Down:
					returnSpeed = STOP_SPEED - speed;
					break;
				case eCameraAction.Up:
					returnSpeed = STOP_SPEED + speed;
					break;
				case eCameraAction.Left:
					returnSpeed = STOP_SPEED - speed;
					break;
				case eCameraAction.Right:
					returnSpeed = STOP_SPEED + speed;
					break;
				case eCameraAction.ZoomIn:
					returnSpeed = STOP_SPEED + speed;
					break;
				case eCameraAction.ZoomOut:
					returnSpeed = STOP_SPEED - speed;
					break;
				default:
					throw new ArgumentOutOfRangeException("action");
			}

			return string.Format("{0:00}", returnSpeed);
		}
	}
}
