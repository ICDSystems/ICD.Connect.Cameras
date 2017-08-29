using System;
using ICD.Common.Properties;
using ICD.Connect.Conferencing.Cameras;

namespace ICD.Connect.Cameras.Panasonic
{
	public static class PanasonicCommandBuilder
	{
		private const int DEFAULT_SPEED = 24;

		[PublicAPI]
		public static string Stop()
		{
			return GetCommandUrl("%23PTS5050");
		}

		[PublicAPI]
		public static string StopZoom()
		{
			return GetCommandUrl("%23Z50");
		}

		[PublicAPI]
		public static string Move(eCameraAction action)
		{
			return Move(action, DEFAULT_SPEED);
		}

		[PublicAPI]
		public static string Move(eCameraAction action, int speed)
		{
			string speedString = GetSpeedStringBasedOnDirection(action, speed);

			switch (action)
			{
				case eCameraAction.Down:
				case eCameraAction.Up:
					return GetCommandUrl(String.Format("%23PTS50{0}", speedString));

				case eCameraAction.Left:
				case eCameraAction.Right:
					return GetCommandUrl(String.Format("%23PTS{0}50", speedString));

				case eCameraAction.ZoomIn:
				case eCameraAction.ZoomOut:
					return GetCommandUrl(String.Format("%23Z{0}", speedString));
				default:
					throw new ArgumentOutOfRangeException("action");
			}
		}

		private static string GetCommandUrl(string command)
		{
			return string.Format("cgi-bin/aw_ptz?cmd={0}&res=1", command);
		}

		private static string GetSpeedStringBasedOnDirection(eCameraAction action, int speed)
		{
			int returnSpeed;

			switch (action)
			{
				case eCameraAction.Down:
					returnSpeed = 50 - speed;
					break;
				case eCameraAction.Up:
					returnSpeed = 50 + speed;
					break;
				case eCameraAction.Left:
					returnSpeed = 50 - speed;
					break;
				case eCameraAction.Right:
					returnSpeed = 50 + speed;
					break;
				case eCameraAction.ZoomIn:
					returnSpeed = 50 + speed;
					break;
				case eCameraAction.ZoomOut:
					returnSpeed = 50 - speed;
					break;
				default:
					throw new ArgumentOutOfRangeException("action");
			}

			return String.Format("{0:00}", returnSpeed);
		}
	}
}
