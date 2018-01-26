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

		#region Public Control API
		[PublicAPI]
		public static string GetPanTiltCommand(int id, eCameraPanTiltAction action)
		{
			return GetPanTiltCommand(id, action, DEFAULT_PAN_SPEED, DEFAULT_TILT_SPEED);
		}

		[PublicAPI]
		public static string GetZoomCommand(int id, eCameraZoomAction action)
		{
			return GetZoomCommand(id, action, DEFAULT_ZOOM_SPEED);
		}

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
			}
			return null;
		}

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
			}
			return null;
		}

		[PublicAPI]
		public static string GetSetAddressCommand()
		{
			return BuildSetAddressCommand();
		}

		[PublicAPI]
		public static string GetClearCommand(int id)
		{
			return BuildClearCommand(id);
		}

		#endregion

		#region Byte Builders
		private static byte GetIdsByte(int sender, int recipient)
		{
			sender = MathUtils.Clamp(sender, 0, 7);
			recipient = MathUtils.Clamp(recipient, 0, 7);

			byte idByte = (byte)(sender << 4);
			idByte += (byte)recipient;
			return idByte.SetBit(7, true);
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
				GetIdsByte(0, id),
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
				GetIdsByte(0, id),
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
				GetIdsByte(0, id), 
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
				GetIdsByte(0, id), 
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
				GetIdsByte(0, id), 
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
				GetIdsByte(0, id),
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
				GetIdsByte(0, id),
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
				GetIdsByte(0, id), 
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

		private static string BuildClearCommand(int id)
		{
			return StringUtils.ToString(new byte[]
			{
				GetIdsByte(0, id), 
				MESSAGE_START_BYTE,
				0x00,
				0x01,
				MESSAGE_END_BYTE
			});
		}
		#endregion
	}
}
