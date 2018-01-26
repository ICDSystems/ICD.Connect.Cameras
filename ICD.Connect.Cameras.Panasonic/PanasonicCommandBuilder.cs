using System;
using System.Text;
using ICD.Common.Properties;

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

		[PublicAPI]
		public static string PanTilt(eCameraPanTiltAction action)
		{
			return PanTilt(action, DEFAULT_SPEED);
		}

		[PublicAPI]
		public static string PanTilt(eCameraPanTiltAction action, int speed)
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
					return Stop();
				default:
					throw new ArgumentOutOfRangeException("action");
			}
		}

		[PublicAPI]
		public static string Zoom(eCameraZoomAction action)
		{
			return Zoom(action, DEFAULT_SPEED);
		}

		[PublicAPI]
		public static string Zoom(eCameraZoomAction action, int speed)
		{
			string speedString = GetSpeedStringBasedOnDirection(action, speed);
			switch (action)
			{
				case eCameraZoomAction.ZoomIn:
				case eCameraZoomAction.ZoomOut:
					return GetCommandUrl(ZOOM, speedString);
				case eCameraZoomAction.Stop:
					return StopZoom();
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
	}
}
				