using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Cameras;

namespace ICD.Connect.Cameras.Visca
{
	public enum eViscaResponse
	{
		OK,
		SYNTAX_ERROR,
		COMMAND_BUFFER_FULL,
		COMMAND_CANCELLED,
		NO_SOCKET,
		NOT_EXECUTABLE,
		UNSPECIFIED,
		NULL,
		IMPROPER_FORMAT
	}

	public static class ViscaResponseHandler
	{
		public static eViscaResponse HandleResponse(string response)
		{
			if (String.IsNullOrEmpty(response))
			{
				response = "Command has no value!";
				return eViscaResponse.NULL;
			}

			byte[] commandComponents = StringUtils.ToBytes(response);

			if (commandComponents.Length > 1)
			{
				//Clear command was sent
				if (commandComponents[0] == '\x88')
				{
					if (commandComponents[1] == '\x30')
						return eViscaResponse.OK;
				}

				//50 is response OK and 41 is Response completed
				if (commandComponents[1] == '\x50' || commandComponents[1] == '\x41')
					return eViscaResponse.OK;

				if (commandComponents[1] == '\x60')
				{
					switch (commandComponents[1])
					{
						case 0x02:
							return eViscaResponse.SYNTAX_ERROR;
						case 0x03:
							return eViscaResponse.COMMAND_BUFFER_FULL;
						case 0x04:
							return eViscaResponse.COMMAND_CANCELLED;
						case 0x05:
							return eViscaResponse.NO_SOCKET;
						case 0x41:
							return eViscaResponse.NOT_EXECUTABLE;
						default:
							return eViscaResponse.UNSPECIFIED;
					}
				}
			}
			else
			{
				//no single legitimate response is less than 2 bytes
				return eViscaResponse.IMPROPER_FORMAT;
			}
			return eViscaResponse.UNSPECIFIED;
		}
	}

	public class ViscaCommandHandler
	{
		private int m_defaultPanSpeed = 8;
		private int m_defaultTiltSpeed = 8;
		private int m_defaultZoomSpeed = 4;

		public void SetDefaultPanSpeed(int panSpeed)
		{
			m_defaultPanSpeed = MathUtils.Clamp(panSpeed, 0, 24);
		}

		public void SetDefaultTiltSpeed(int tiltSpeed)
		{
			m_defaultTiltSpeed = MathUtils.Clamp(tiltSpeed, 0, 24);
		}

		public void SetDefaultZoomSpeed(int zoomSpeed)
		{
			m_defaultZoomSpeed = MathUtils.Clamp(zoomSpeed, 0, 7);
		}

		public static byte GetIdsByte(int sender, int recipient)
		{
			sender = MathUtils.Clamp(sender, 0, 7);
			recipient = MathUtils.Clamp(recipient, 0, 7);

			byte idByte = (byte)(sender << 4);
			idByte += (byte)recipient;
			return idByte.SetBit(7, true);
		}

		private byte GetPanSpeedByte(int speed)
		{
			speed = MathUtils.Clamp(speed, 0, 24);
			return (byte)speed;
		}

		private byte GetTiltSpeed(int speed)
		{
			speed = MathUtils.Clamp(speed, 0, 24);
			return (byte)speed;
		}

		private byte GetZoomInSpeed(int speed)
		{
			speed = MathUtils.Clamp(speed, 0, 7);
			return (byte)(speed + 32);
		}

		private byte GetZoomOutSpeed(int speed)
		{
			speed = MathUtils.Clamp(speed, 0, 7);
			return (byte)(speed + 48);
		}

		//public methods that do things

		public string SetAddress()
		{
			return StringUtils.ToString(new byte[] {0x88, 0x30, 0x01, 0xFF});
		}

		public string Clear(int Id)
		{
			return StringUtils.ToString(new byte[] {GetIdsByte(0, Id), 0x01, 0x00, 0x01, 0xFF});
		}

		public string Stop(int Id)
		{
			return
				StringUtils.ToString(new byte[]
				{GetIdsByte(0, Id), 0x01, 0x04, 0x07, 0x00, 0xFF, GetIdsByte(0, Id), 0x01, 0x06, 0x01, 0x01, 0x01, 0x03, 0x03, 0xFF});
		}

		/// <summary>
		/// Moves camera with default values
		/// </summary>
		/// <param name="Id"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public string Move(int Id, eCameraAction action)
		{
			//Move with defaults
			return Move(Id, action, m_defaultPanSpeed, m_defaultTiltSpeed, m_defaultZoomSpeed);
		}

		/// <summary>
		/// Overloaded function used to move camera.
		/// </summary>
		/// <param name="Id"></param>
		/// <param name="action"></param>
		/// <param name="panSpeed"></param>
		/// <param name="tiltSpeed"></param>
		/// <returns></returns>
		public string Move(int Id, eCameraAction action, int panSpeed, int tiltSpeed)
		{
			return Move(Id, action, panSpeed, tiltSpeed, 1);
		}

		/// <summary>
		/// Moves the camera or zooms in, based on eCameraAction.
		/// </summary>
		/// <param name="Id"></param>
		/// <param name="action"></param>
		/// <param name="panSpeed"></param>
		/// <param name="tiltSpeed"></param>
		/// <param name="zoomSpeed"></param>
		/// <returns></returns>
		public string Move(int Id, eCameraAction action, int panSpeed, int tiltSpeed, int zoomSpeed)
		{
			switch (action)
			{
				case eCameraAction.Up:
					return
						StringUtils.ToString(new byte[]
						{GetIdsByte(0, Id), 0x01, 0x06, 0x01, GetPanSpeedByte(panSpeed), GetTiltSpeed(tiltSpeed), 0x03, 0x01, 0xFF});
				case eCameraAction.Down:
					return
						StringUtils.ToString(new byte[]
						{GetIdsByte(0, Id), 0x01, 0x06, 0x01, GetPanSpeedByte(panSpeed), GetTiltSpeed(tiltSpeed), 0x03, 0x02, 0xFF});
				case eCameraAction.Left:
					return
						StringUtils.ToString(new byte[]
						{GetIdsByte(0, Id), 0x01, 0x06, 0x01, GetPanSpeedByte(panSpeed), GetTiltSpeed(tiltSpeed), 0x01, 0x03, 0xFF});
				case eCameraAction.Right:
					return
						StringUtils.ToString(new byte[]
						{GetIdsByte(0, Id), 0x01, 0x06, 0x01, GetPanSpeedByte(panSpeed), GetTiltSpeed(tiltSpeed), 0x02, 0x03, 0xFF});
				case eCameraAction.ZoomIn:
					return StringUtils.ToString(new byte[] {GetIdsByte(0, Id), 0x01, 0x04, 0x07, GetZoomInSpeed(zoomSpeed), 0xFF});
				case eCameraAction.ZoomOut:
					return StringUtils.ToString(new byte[] {GetIdsByte(0, Id), 0x01, 0x04, 0x07, GetZoomOutSpeed(zoomSpeed), 0xFF});
			}
			return null;
		}
	}
}
