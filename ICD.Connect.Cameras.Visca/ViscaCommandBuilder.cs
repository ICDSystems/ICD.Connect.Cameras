using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Cameras.Visca
{
	public static class ViscaCommandBuilder
	{
		#region Byte Constants
		private const byte MESSAGE_START_BYTE = 0x01;
		private const byte MESSAGE_END_BYTE = 0xFF;
		#endregion

		#region Default Speeds

		private const int DEFAULT_PAN_SPEED = 8;
		private const int DEFAULT_TILT_SPEED = 8;
		private const int DEFAULT_ZOOM_SPEED = 4;
		#endregion

		#region Public Commands
		/// <summary>
		/// Gets the Pan/Tilt Command, using the default speed
		/// </summary>
		/// <param name="id">The sequential Id of the camera to perform the operation on.</param>
		/// <param name="action">The Pan/Tilt action desired.</param>
		[PublicAPI]
		public static string GetPanTiltCommand(int id, eCameraPanTiltAction action)
		{
			return GetPanTiltCommand(id, action, DEFAULT_PAN_SPEED, DEFAULT_TILT_SPEED);
		}

		/// <summary>
		/// Gets the Zoom Command URL, using the default provided.
		/// </summary>
		/// <param name="id">The sequential Id of the camera to perform the operation on.</param>
		/// <param name="action">The Zoom action desired.</param>
		[PublicAPI]
		public static string GetZoomCommand(int id, eCameraZoomAction action)
		{
			return GetZoomCommand(id, action, DEFAULT_ZOOM_SPEED);
		}

		/// <summary>
		/// Gets the Pan/Tilt Command, using the speed provided.
		/// </summary>
		/// <param name="id">The sequential Id of the camera to perform the operation on.</param>
		/// <param name="action">The Pan/Tilt action desired.</param>
		/// <param name="panSpeed">The desired speed for panning.</param>
		/// <param name="tiltSpeed">The desired speed for tilting.</param>
		[PublicAPI]
		public static string GetPanTiltCommand(int id, eCameraPanTiltAction action, int panSpeed, int tiltSpeed)
		{
			switch (action)
			{
				case eCameraPanTiltAction.Up:
					return BuildUpCommand(id, panSpeed, tiltSpeed);
				case eCameraPanTiltAction.Down:
					return BuildDownCommand(id, panSpeed, tiltSpeed);
				case eCameraPanTiltAction.Left:
					return BuildLeftCommand(id, panSpeed, tiltSpeed);
				case eCameraPanTiltAction.Right:
					return BuildRightCommand(id, panSpeed, tiltSpeed);
				case eCameraPanTiltAction.Stop:
					return BuildStopPanTiltCommand(id);
				default:
					throw new ArgumentOutOfRangeException("action");
			}
		}

		/// <summary>
		/// Gets the Zoom Command URL, using the speed provided.
		/// </summary>
		/// <param name="id">The sequential Id of the camera to perform the operation on.</param>
		/// <param name="action">The Zoom action desired.</param>
		/// <param name="zoomSpeed">The desired speed, where 0 is still and 50 is fastest possible</param>
		[PublicAPI]
		public static string GetZoomCommand(int id, eCameraZoomAction action, int zoomSpeed)
		{
			switch (action)
			{
				case eCameraZoomAction.ZoomIn:
					return BuildZoomInCommand(id, zoomSpeed);
				case eCameraZoomAction.ZoomOut:
					return BuildZoomOutCommand(id, zoomSpeed);
				case eCameraZoomAction.Stop:
					return BuildStopZoomCommand(id);
				default:
					throw new ArgumentOutOfRangeException("action");
			}
		}

		/// <summary>
		/// Gets the command which tells all VISCA cameras in a chain of VISCA cameras to discover their own ids.
		/// </summary>
		[PublicAPI]
		public static string GetSetAddressCommand()
		{
			return BuildSetAddressCommand();
		}

		/// <summary>
		/// Gets the command to clear all pending commands for the given camera.
		/// </summary>
		[PublicAPI]
		public static string GetClearCommand()
		{
			return BuildClearCommand();
		}

		/// <summary>
		/// Gets the command to wake the camera.
		/// </summary>
		/// <param name="id">The sequential id of the camera to power on</param>
		/// <returns></returns>
		[PublicAPI]
		public static string GetPowerOnCommand(int id)
		{
			return BuildPowerOnCommand(id);
		}


		/// <summary>
		/// Gets the command to park the camera facing the wall and disable video.
		/// </summary>
		/// <param name="id">The sequential id of the camera to power off.</param>
		/// <returns></returns>
		[PublicAPI]
		public static string GetPowerOffCommand(int id)
		{
			return BuildPowerOffCommand(id);
		}

		#endregion

		#region Byte Builders
		private static byte GetIdsByte( int recipient)
		{
			recipient = MathUtils.Clamp(recipient, 1, 7);
			return (byte)(0x80 + recipient);
		}

		private static byte GetPanSpeedByte(int speed)
		{
			speed = MathUtils.Clamp(speed, 0, 24);
			return (byte)speed;
		}

		private static byte GetTiltSpeedByte(int speed)
		{
			speed = MathUtils.Clamp(speed, 0, 24);
			return (byte)speed;
		}

		private static byte GetZoomInSpeedByte(int speed)
		{
			speed = MathUtils.Clamp(speed, 0, 7);
			return (byte)(speed + 32);
		}

		private static byte GetZoomOutSpeedByte(int speed)
		{
			speed = MathUtils.Clamp(speed, 0, 7);
			return (byte)(speed + 48);
		}
		#endregion

		#region Command Builders

		private static string BuildStopPanTiltCommand(int id)
		{
			return StringUtils.ToString(new byte[]
			{
				GetIdsByte(id),
				MESSAGE_START_BYTE,
				0x06,
				0x01,
				0x01,
				0x01,
				0x03,
				0x03,
				MESSAGE_END_BYTE
			});

		}

		private static string BuildUpCommand(int id, int panSpeed, int tiltSpeed)
		{
			return StringUtils.ToString(new byte[]
			{
				GetIdsByte(id),
				MESSAGE_START_BYTE,
				0x06,
				0x01,
				GetPanSpeedByte(panSpeed),
				GetTiltSpeedByte(tiltSpeed),
				0x03,
				0x01,
				MESSAGE_END_BYTE
			});
		}

		private static string BuildDownCommand(int id, int panSpeed, int tiltSpeed)
		{
			return StringUtils.ToString(new byte[]
			{
				GetIdsByte(id),
				MESSAGE_START_BYTE,
				0x06,
				0x01,
				GetPanSpeedByte(panSpeed),
				GetTiltSpeedByte(tiltSpeed),
				0x03,
				0x02,
				MESSAGE_END_BYTE
			});

		}

		private static string BuildLeftCommand(int id, int panSpeed, int tiltSpeed)
		{
			return StringUtils.ToString(new byte[]
			{
				GetIdsByte(id),
				MESSAGE_START_BYTE,
				0x06,
				0x01,
				GetPanSpeedByte(panSpeed),
				GetTiltSpeedByte(tiltSpeed),
				0x01,
				0x03,
				MESSAGE_END_BYTE
			});

		}

		private static string BuildRightCommand(int id, int panSpeed, int tiltSpeed)
		{
			return StringUtils.ToString(new byte[]
			{
				GetIdsByte(id),
				MESSAGE_START_BYTE,
				0x06,
				0x01,
				GetPanSpeedByte(panSpeed),
				GetTiltSpeedByte(tiltSpeed),
				0x02,
				0x03,
				MESSAGE_END_BYTE
			});

		}

		private static string BuildStopZoomCommand(int id)
		{
			return StringUtils.ToString(new byte[]
			{
				GetIdsByte(id),
				MESSAGE_START_BYTE,
				0x04,
				0x07,
				0x00,
				MESSAGE_END_BYTE
			});
		}

		private static string BuildZoomInCommand(int id, int zoomSpeed)
		{
			return StringUtils.ToString(new byte[]
			{
				GetIdsByte(id),
				MESSAGE_START_BYTE,
				0x04,
				0x07,
				GetZoomInSpeedByte(zoomSpeed),
				MESSAGE_END_BYTE
			});
		}

		private static string BuildZoomOutCommand(int id, int zoomSpeed)
		{
			return StringUtils.ToString(new byte[]
			{
				GetIdsByte(id),
				MESSAGE_START_BYTE,
				0x04,
				0x07,
				GetZoomOutSpeedByte(zoomSpeed),
				MESSAGE_END_BYTE
			});
		}

		private static string BuildSetAddressCommand()
		{
			return StringUtils.ToString(new byte[]
			{
				0x88,
				0x30,
				0x01,
				MESSAGE_END_BYTE
			});
		}

		private static string BuildClearCommand()
		{
			return StringUtils.ToString(new byte[]
			{
				0x88,
				MESSAGE_START_BYTE,
				0x00,
				0x01,
				MESSAGE_END_BYTE
			});
		}

		private static string BuildPowerOnCommand(int id)
		{
			return StringUtils.ToString(new byte[]
			{
				GetIdsByte(id),
				MESSAGE_START_BYTE,
				0x04,
				0x00,
				0x02,
				MESSAGE_END_BYTE
			});
		}

		private static string BuildPowerOffCommand(int id)
		{
			return StringUtils.ToString(new byte[]
			{
				GetIdsByte(id),
				MESSAGE_START_BYTE,
				0x04,
				0x00,
				0x03,
				MESSAGE_END_BYTE
			});
		}
		#endregion
	}
}
