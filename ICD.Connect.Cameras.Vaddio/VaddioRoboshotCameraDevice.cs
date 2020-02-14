using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Protocol;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Settings;

namespace ICD.Connect.Cameras.Vaddio
{
	public sealed class VaddioRoboshotCameraDevice : AbstractCameraDevice<VaddioRoboshotCameraDeviceSettings>, IDeviceWithPower
	{
		#region Constants

		private const char DELIMITER = '\r';

		private const int DEFAULT_PAN_SPEED = 12;
		private const int DEFAULT_TILT_SPEED = 10;
		private const int DEFAULT_ZOOM_SPEED = 3;

		private const string DEFAULT_USERNAME = "admin";
		private const string DEFAULT_PASSWORD = "password";

		#endregion

		#region Private Members

		private readonly ConnectionStateManager m_ConnectionStateManager;
		private readonly VaddioRoboshotSerialBuffer m_SerialBuffer;

		private readonly NetworkProperties m_NetworkProperties;

		private int m_PanSpeed;
		private int m_TiltSpeed;
		private int m_ZoomSpeed;

		#endregion

		#region Properties

		/// <summary>
		/// Username for telnet authentication.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Password for telnet authentication.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Gets the maximum number of presets this camera can support.
		/// </summary>
		public override int MaxPresets { get { return 16; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public VaddioRoboshotCameraDevice()
		{
			m_NetworkProperties = new NetworkProperties();

			m_SerialBuffer = new VaddioRoboshotSerialBuffer();
			Subscribe(m_SerialBuffer);

			m_ConnectionStateManager = new ConnectionStateManager(this) {ConfigurePort = ConfigurePort};
			m_ConnectionStateManager.OnSerialDataReceived += PortOnSerialDataReceived;
			m_ConnectionStateManager.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
			m_ConnectionStateManager.OnConnectedStateChanged += PortOnConnectedStateChanged;

			Controls.Add(new GenericCameraRouteSourceControl<VaddioRoboshotCameraDevice>(this, 0));
			Controls.Add(new CameraDeviceControl(this, 1));
			Controls.Add(new PowerDeviceControl<VaddioRoboshotCameraDevice>(this, 2));
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			m_ConnectionStateManager.OnIsOnlineStateChanged -= PortOnIsOnlineStateChanged;
			m_ConnectionStateManager.Dispose();

			base.DisposeFinal(disposing);

			Unsubscribe(m_SerialBuffer);
		}

		/// <summary>
		/// Called when we receive data from the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void PortOnSerialDataReceived(object sender, StringEventArgs stringEventArgs)
		{
			m_SerialBuffer.Enqueue(stringEventArgs.Data);
		}

		/// <summary>
		/// Called when the port online status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PortOnIsOnlineStateChanged(object sender, BoolEventArgs e)
		{
			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Called when we gain/lose connection to the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void PortOnConnectedStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			m_SerialBuffer.Clear();
		}

		#region Methods

		/// <summary>
		/// Sets the port for communicating with the physical device.
		/// </summary>
		/// <param name="port"></param>
		public void SetPort(ISerialPort port)
		{
			m_ConnectionStateManager.SetPort(port);
		}

		/// <summary>
		/// Configures the given port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		private void ConfigurePort(ISerialPort port)
		{
			// TCP
			if (port is INetworkPort)
				(port as INetworkPort).ApplyDeviceConfiguration(m_NetworkProperties);
		}

		/// <summary>
		/// Begins panning the camera
		/// </summary>
		/// <param name="action"></param>
		public override void Pan(eCameraPanAction action)
		{
			string command;

			switch (action)
			{
				case eCameraPanAction.Left:
					command = string.Format("camera pan left {0}", m_PanSpeed);
					break;
				case eCameraPanAction.Right:
					command = string.Format("camera pan right {0}", m_PanSpeed);
					break;
				case eCameraPanAction.Stop:
					command = "camera pan stop";
					break;

				default:
					throw new ArgumentOutOfRangeException("action");
			}

			SendCommand(command);
		}

		/// <summary>
		/// Begin tilting the camera.
		/// </summary>
		public override void Tilt(eCameraTiltAction action)
		{
			string command;

			switch (action)
			{
				case eCameraTiltAction.Up:
					command = string.Format("camera tilt up {0}", m_TiltSpeed);
					break;
				case eCameraTiltAction.Down:
					command = string.Format("camera tilt down {0}", m_TiltSpeed);
					break;
				case eCameraTiltAction.Stop:
					command = "camera tilt stop";
					break;

				default:
					throw new ArgumentOutOfRangeException("action");
			}

			SendCommand(command);
		}

		/// <summary>
		/// Starts zooming the camera with the given action.
		/// </summary>
		/// <param name="action"></param>
		public override void Zoom(eCameraZoomAction action)
		{
			string command;

			switch (action)
			{
				case eCameraZoomAction.ZoomIn:
					command = string.Format("camera zoom in {0}", m_ZoomSpeed);
					break;
				case eCameraZoomAction.ZoomOut:
					command = string.Format("camera zoom out {0}", m_ZoomSpeed);
					break;
				case eCameraZoomAction.Stop:
					command = "camera zoom stop";
					break;

				default:
					throw new ArgumentOutOfRangeException("action");
			}

			SendCommand(command);
		}

		/// <summary>
		/// Gets the stored camera presets.
		/// </summary>
		public override IEnumerable<CameraPreset> GetPresets()
		{
			return Enumerable.Range(1, 16).Select(i => new CameraPreset(i, string.Format("Preset {0}", i)));
		}

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		public override void ActivatePreset(int presetId)
		{
			string command = string.Format("camera preset recall {0}", presetId);
			SendCommand(command);
		}

		/// <summary>
		/// Stores the cameras current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		public override void StorePreset(int presetId)
		{
			string command = string.Format("camera preset store {0}", presetId);
			SendCommand(command);
		}

		/// <summary>
		/// Sets if the camera mute state should be active
		/// </summary>
		/// <param name="enable"></param>
		public override void MuteCamera(bool enable)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Resets camera to its predefined home position
		/// </summary>
		public override void SendCameraHome()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Powers on the device.
		/// </summary>
		public void PowerOn()
		{
			SendCommand("camera standby off");
		}

		/// <summary>
		/// Powers off the device.
		/// </summary>
		public void PowerOff()
		{
			SendCommand("camera standby on");
		}

		[PublicAPI]
		public void SendCommand(string command)
		{
			if (!command.EndsWith(DELIMITER))
				command += DELIMITER;

			m_ConnectionStateManager.Send(command);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_ConnectionStateManager != null && m_ConnectionStateManager.IsOnline;
		}

		#endregion

		#region SerialBuffer Callbacks

		private void Subscribe(VaddioRoboshotSerialBuffer buffer)
		{
			buffer.OnCompletedSerial += BufferOnCompletedSerial;
			buffer.OnPasswordPrompt += BufferOnPasswordPrompt;
			buffer.OnUsernamePrompt += BufferOnUsernamePrompt;
		}

		private void Unsubscribe(VaddioRoboshotSerialBuffer buffer)
		{
			buffer.OnCompletedSerial -= BufferOnCompletedSerial;
			buffer.OnPasswordPrompt -= BufferOnPasswordPrompt;
			buffer.OnUsernamePrompt -= BufferOnUsernamePrompt;
		}

		private void BufferOnUsernamePrompt(object sender, EventArgs eventArgs)
		{
			SendCommand(Username ?? string.Empty);
		}

		private void BufferOnPasswordPrompt(object sender, EventArgs eventArgs)
		{
			SendCommand(Password ?? string.Empty);
		}

		private void BufferOnCompletedSerial(object sender, StringEventArgs stringEventArgs)
		{
			const string responseRegex = @"(?'command'[^\r\n]+)\r\n((?'message'[^\r\n]+)\r\n)?(?'status'[^\r\n]+)\r\n";

			Match match = Regex.Match(stringEventArgs.Data, responseRegex);

			if (!match.Success)
				return;

			if(match.Groups["status"].Value.Equals("ERROR"))
				Log(eSeverity.Error, "Error executing \"{0}\" - {1}", match.Groups["command"].Value, match.Groups["message"].Value);
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(VaddioRoboshotCameraDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Username = Username;
			settings.Password = Password;

			settings.PanSpeed = m_PanSpeed;
			settings.TiltSpeed = m_TiltSpeed;
			settings.ZoomSpeed = m_ZoomSpeed;

			settings.Port = m_ConnectionStateManager.PortNumber;

			settings.Copy(m_NetworkProperties);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Username = null;
			Password = null;

			m_PanSpeed = DEFAULT_PAN_SPEED;
			m_TiltSpeed = DEFAULT_TILT_SPEED;
			m_ZoomSpeed = DEFAULT_ZOOM_SPEED;

			m_NetworkProperties.ClearNetworkProperties();

			SupportedCameraFeatures = eCameraFeatures.None;

			SetPort(null);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(VaddioRoboshotCameraDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_NetworkProperties.Copy(settings);

			Username = settings.Username ?? DEFAULT_USERNAME;
			Password = settings.Password ?? DEFAULT_PASSWORD;

			m_PanSpeed = settings.PanSpeed ?? DEFAULT_PAN_SPEED;
			m_TiltSpeed = settings.TiltSpeed ?? DEFAULT_TILT_SPEED;
			m_ZoomSpeed = settings.ZoomSpeed ?? DEFAULT_ZOOM_SPEED;

			ISerialPort port = null;

			if (settings.Port != null)
			{
				try
				{
					port = factory.GetPortById((int)settings.Port) as ISerialPort;
				}
				catch (KeyNotFoundException)
				{
					Log(eSeverity.Error, "No serial port with Id {0}", settings.Port);
				}
			}

			SetPort(port);

			SupportedCameraFeatures = eCameraFeatures.PanTiltZoom | eCameraFeatures.Presets;
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("PowerOn", "Powers the camera device", () => PowerOn());
			yield return new ConsoleCommand("PowerOff", "Places the camera device on standby", () => PowerOff());
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
