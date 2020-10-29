using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Connect.Protocol.Data;

namespace ICD.Connect.Cameras.Visca
{
	public sealed class ViscaCommand : ISerialData
	{
		#region Byte Constants

		private const byte MESSAGE_START_BYTE = 0x01;
		private const byte MESSAGE_END_BYTE = 0xFF;

		private const byte INQUIRY_START_BYTE = 0x09;

		#endregion

		#region Default Speeds

		private const int DEFAULT_PAN_SPEED = 8;
		private const int DEFAULT_TILT_SPEED = 8;
		private const int DEFAULT_ZOOM_SPEED = 4;

		#endregion

		[NotNull]
		private readonly byte[] m_Bytes;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="bytes"></param>
		public ViscaCommand([NotNull] params byte[] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException("bytes");

			m_Bytes = bytes.ToArray();
		}

		/// <summary>
		/// Gets the Pan Command, using the default speed
		/// </summary>
		/// <param name="id">The sequential Id of the camera to perform the operation on.</param>
		/// <param name="action">The Pan action desired.</param>
		[PublicAPI]
		public static ViscaCommand GetPanCommand(int id, eCameraPanAction action)
		{
			return GetPanCommand(id, action, DEFAULT_PAN_SPEED);
		}

		/// <summary>
		/// Gets the Tilt Command, using the default speed
		/// </summary>
		/// <param name="id">The sequential Id of the camera to perform the operation on.</param>
		/// <param name="action">The Tilt action desired.</param>
		[PublicAPI]
		public static ViscaCommand GetTiltCommand(int id, eCameraTiltAction action)
		{
			return GetTiltCommand(id, action, DEFAULT_TILT_SPEED);
		}

		/// <summary>
		/// Gets the Zoom Command URL, using the default provided.
		/// </summary>
		/// <param name="id">The sequential Id of the camera to perform the operation on.</param>
		/// <param name="action">The Zoom action desired.</param>
		[PublicAPI]
		public static ViscaCommand GetZoomCommand(int id, eCameraZoomAction action)
		{
			return GetZoomCommand(id, action, DEFAULT_ZOOM_SPEED);
		}

		/// <summary>
		/// Gets the Pan Command, using the speed provided.
		/// </summary>
		/// <param name="id">The sequential Id of the camera to perform the operation on.</param>
		/// <param name="action">The Pan action desired.</param>
		/// <param name="panSpeed">The desired speed for panning.</param>
		[PublicAPI]
		public static ViscaCommand GetPanCommand(int id, eCameraPanAction action, int panSpeed)
		{
			switch (action)
			{
				case eCameraPanAction.Left:
					return BuildLeftCommand(id, panSpeed);
				case eCameraPanAction.Right:
					return BuildRightCommand(id, panSpeed);
				case eCameraPanAction.Stop:
					return BuildStopPanTiltCommand(id);
				default:
					throw new ArgumentOutOfRangeException("action");
			}
		}

		/// <summary>
		/// Gets the Tilt Command, using the speed provided.
		/// </summary>
		/// <param name="id">The sequential Id of the camera to perform the operation on.</param>
		/// <param name="action">The Tilt action desired.</param>
		/// <param name="tiltSpeed">The desired speed for tilting.</param>
		[PublicAPI]
		public static ViscaCommand GetTiltCommand(int id, eCameraTiltAction action, int tiltSpeed)
		{
			switch (action)
			{
				case eCameraTiltAction.Up:
					return BuildUpCommand(id, tiltSpeed);
				case eCameraTiltAction.Down:
					return BuildDownCommand(id, tiltSpeed);
				case eCameraTiltAction.Stop:
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
		public static ViscaCommand GetZoomCommand(int id, eCameraZoomAction action, int zoomSpeed)
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
		public static ViscaCommand GetSetAddressCommand()
		{
			return new ViscaCommand(
				0x88,
				0x30,
				0x01,
				MESSAGE_END_BYTE
				);
		}

		/// <summary>
		/// Gets the command to clear all pending commands for the given camera.
		/// </summary>
		[PublicAPI]
		public static ViscaCommand GetClearCommand()
		{
			return new ViscaCommand(
				0x88,
				MESSAGE_START_BYTE,
				0x00,
				0x01,
				MESSAGE_END_BYTE
				);
		}

		/// <summary>
		/// Gets the command to wake the camera.
		/// </summary>
		/// <param name="id">The sequential id of the camera to power on</param>
		/// <returns></returns>
		[PublicAPI]
		public static ViscaCommand GetPowerOnCommand(int id)
		{
			return new ViscaCommand(
				GetIdByte(id),
				MESSAGE_START_BYTE,
				0x04,
				0x00,
				0x02,
				MESSAGE_END_BYTE
				);
		}

		/// <summary>
		/// Gets the command to park the camera facing the wall and disable video.
		/// </summary>
		/// <param name="id">The sequential id of the camera to power off.</param>
		/// <returns></returns>
		[PublicAPI]
		public static ViscaCommand GetPowerOffCommand(int id)
		{
			return new ViscaCommand(
				GetIdByte(id),
				MESSAGE_START_BYTE,
				0x04,
				0x00,
				0x03,
				MESSAGE_END_BYTE
				);
		}

		/// <summary>
		/// Gets the command to request the current power state of the camera.
		/// </summary>
		/// <param name="id">The sequential id of the camera to power off.</param>
		/// <returns></returns>
		public static ViscaCommand GetPowerInquiryCommand(int id)
		{
			return new ViscaCommand(
				GetIdByte(id),
				INQUIRY_START_BYTE,
				0x04,
				0x00,
				MESSAGE_END_BYTE
				);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Serialize this instance to a string.
		/// </summary>
		/// <returns></returns>
		public string Serialize()
		{
			return StringUtils.ToString(m_Bytes);
		}

		/// <summary>
		/// Returns true if the bytes for this command match the bytes for the given command.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool CommandEquals([NotNull] ViscaCommand other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			return m_Bytes.SequenceEqual(other.m_Bytes);
		}

		#endregion

		#region Byte Builders

		private static byte GetIdByte(int recipient)
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

		private static ViscaCommand BuildStopPanTiltCommand(int id)
		{
			return new ViscaCommand(
				GetIdByte(id),
				MESSAGE_START_BYTE,
				0x06,
				0x01,
				0x01,
				0x01,
				0x03,
				0x03,
				MESSAGE_END_BYTE
			);
		}

		private static ViscaCommand BuildUpCommand(int id, int tiltSpeed)
		{
			return new ViscaCommand(
				GetIdByte(id),
				MESSAGE_START_BYTE,
				0x06,
				0x01,
				GetPanSpeedByte(DEFAULT_PAN_SPEED),
				GetTiltSpeedByte(tiltSpeed),
				0x03,
				0x01,
				MESSAGE_END_BYTE
			);
		}

		private static ViscaCommand BuildDownCommand(int id, int tiltSpeed)
		{
			return new ViscaCommand(
				GetIdByte(id),
				MESSAGE_START_BYTE,
				0x06,
				0x01,
				GetPanSpeedByte(DEFAULT_PAN_SPEED),
				GetTiltSpeedByte(tiltSpeed),
				0x03,
				0x02,
				MESSAGE_END_BYTE
			);
		}

		private static ViscaCommand BuildLeftCommand(int id, int panSpeed)
		{
			return new ViscaCommand(
				GetIdByte(id),
				MESSAGE_START_BYTE,
				0x06,
				0x01,
				GetPanSpeedByte(panSpeed),
				GetTiltSpeedByte(DEFAULT_TILT_SPEED),
				0x01,
				0x03,
				MESSAGE_END_BYTE
			);
		}

		private static ViscaCommand BuildRightCommand(int id, int panSpeed)
		{
			return new ViscaCommand(
				GetIdByte(id),
				MESSAGE_START_BYTE,
				0x06,
				0x01,
				GetPanSpeedByte(panSpeed),
				GetTiltSpeedByte(DEFAULT_TILT_SPEED),
				0x02,
				0x03,
				MESSAGE_END_BYTE
			);
		}

		private static ViscaCommand BuildStopZoomCommand(int id)
		{
			return new ViscaCommand(
				GetIdByte(id),
				MESSAGE_START_BYTE,
				0x04,
				0x07,
				0x00,
				MESSAGE_END_BYTE
			);
		}

		private static ViscaCommand BuildZoomInCommand(int id, int zoomSpeed)
		{
			return new ViscaCommand(
				GetIdByte(id),
				MESSAGE_START_BYTE,
				0x04,
				0x07,
				GetZoomInSpeedByte(zoomSpeed),
				MESSAGE_END_BYTE
			);
		}

		private static ViscaCommand BuildZoomOutCommand(int id, int zoomSpeed)
		{
			return new ViscaCommand(
				GetIdByte(id),
				MESSAGE_START_BYTE,
				0x04,
				0x07,
				GetZoomOutSpeedByte(zoomSpeed),
				MESSAGE_END_BYTE
			);
		}

		#endregion
	}
}
