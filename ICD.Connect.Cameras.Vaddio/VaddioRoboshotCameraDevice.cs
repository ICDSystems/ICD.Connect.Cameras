using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Controls.Power;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Protocol;
using ICD.Connect.Protocol.Data;
using ICD.Connect.Protocol.EventArguments;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports;
using ICD.Connect.Protocol.Network.Ports.Tcp;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.SerialQueues;
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

		/// <summary>
		/// Raised when the powered state changes.
		/// </summary>
		public event EventHandler<PowerDeviceControlPowerStateApiEventArgs> OnPowerStateChanged;

		#region Private Members

		private readonly ConnectionStateManager m_ConnectionStateManager;
		private readonly VaddioRoboshotSerialBuffer m_SerialBuffer;
		private readonly NetworkProperties m_NetworkProperties;

		private int m_PanSpeed;
		private int m_TiltSpeed;
		private int m_ZoomSpeed;
		private ePowerState m_PowerState;
		private ISerialQueue m_SerialQueue;

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
		public override int MaxPresets
		{
			get
			{
				// Hack - There is no way to store the home position, so we are using preset 16 as our home
				// return 16;
				return 15;
			}
		}

		/// <summary>
		/// Gets the powered state of the device.
		/// </summary>
		public ePowerState PowerState
		{
			get { return m_PowerState; }
			private set
			{
				if (value == m_PowerState)
					return;

				m_PowerState = value;

				OnPowerStateChanged.Raise(this, new PowerDeviceControlPowerStateApiEventArgs(m_PowerState));
			}
		}

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
			m_ConnectionStateManager.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
			m_ConnectionStateManager.OnConnectedStateChanged += PortOnConnectedStateChanged;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnPowerStateChanged = null;

			m_ConnectionStateManager.OnIsOnlineStateChanged -= PortOnIsOnlineStateChanged;
			m_ConnectionStateManager.Dispose();

			base.DisposeFinal(disposing);

			Unsubscribe(m_SerialBuffer);
		}

		#region Methods

		/// <summary>
		/// Sets the port for communicating with the physical device.
		/// </summary>
		/// <param name="port"></param>
		public void SetPort(ISerialPort port)
		{
			if (port != null)
			{
				port.DebugRx = eDebugMode.Ascii;
				port.DebugTx = eDebugMode.Ascii;
			}

			m_ConnectionStateManager.SetPort(port, false);
		}

		/// <summary>
		/// Configures the given port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		private void ConfigurePort(IPort port)
		{
			// TCP
			if (port is INetworkPort)
				(port as INetworkPort).ApplyDeviceConfiguration(m_NetworkProperties);

			SerialQueue queue = new SerialQueue();
			queue.SetPort(port as ISerialPort);
			queue.SetBuffer(m_SerialBuffer);
			queue.Timeout = 3 * 1000;

			SetSerialQueue(queue);

			UpdateCachedOnlineStatus();
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
		public override void ActivateHome()
		{
			// Hack - There is no way to store the home position, so we are using preset 16 as our home
			//SendCommand("camera home");
			ActivatePreset(16);
		}

		/// <summary>
		/// Stores the current position as the home position.
		/// </summary>
		public override void StoreHome()
		{
			// Hack - There is no way to store the home position, so we are using preset 16 as our home
			StorePreset(16);
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

			m_SerialQueue.Enqueue(new SerialData(command));
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

		#region SerialQueue Callbacks

		/// <summary>
		/// Subscribe to the serial queue events.
		/// </summary>
		/// <param name="serialQueue"></param>
		private void SetSerialQueue(ISerialQueue serialQueue)
		{
			Unsubscribe(m_SerialQueue);

			if (m_SerialQueue != null)
				m_SerialQueue.Dispose();

			m_SerialQueue = serialQueue;

			Subscribe(m_SerialQueue);

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Subscribe to the serial queue events.
		/// </summary>
		/// <param name="queue"></param>
		private void Subscribe(ISerialQueue queue)
		{
			if (queue == null)
				return;

			queue.OnSerialResponse += SerialQueueOnSerialResponse;
		}

		/// <summary>
		/// Unsubscribe from the serial queue events.
		/// </summary>
		/// <param name="queue"></param>
		private void Unsubscribe(ISerialQueue queue)
		{
			if (queue == null)
				return;

			queue.OnSerialResponse -= SerialQueueOnSerialResponse;
		}

		/// <summary>
		/// Called when we receive a response from the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SerialQueueOnSerialResponse(object sender, SerialResponseEventArgs args)
		{
			// Initialize
			if (args.Response.Contains("Welcome admin"))
				SendCommand("camera standby get");

			const string responseRegex = @"\s*(?'command'[^\r\n]+)\r\n((?'message'[^\r\n]+)\r\n)?(?'status'[^\r\n]+)\r\n";

			Match match = Regex.Match(args.Response, responseRegex);
			if (!match.Success)
				return;

			string command = match.Groups["command"].Value;
			string status = match.Groups["status"].Value;
			string message = match.Groups["message"].Value;

			switch (status)
			{
				case "ERROR":
					Logger.Log(eSeverity.Error, "Error executing \"{0}\" - {1}", command, message);
					break;

				case "OK":
					switch (command)
					{
						case "camera standby on":
							PowerState = ePowerState.PowerOff;
							break;

						case "camera standby off":
							PowerState = ePowerState.PowerOn;
							break;

						case "camera standby get":
							PowerState = message.Contains("on") ? ePowerState.PowerOff : ePowerState.PowerOn;
							break;
					}

					break;
			}
		}

		#endregion

		#region Port Callbacks

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
		/// <param name="eventArgs"></param>
		private void PortOnConnectedStateChanged(object sender, BoolEventArgs eventArgs)
		{
			m_SerialBuffer.Clear();
		}

		#endregion

		#region SerialBuffer Callbacks

		/// <summary>
		/// Subscribe to the buffer events.
		/// </summary>
		/// <param name="buffer"></param>
		private void Subscribe(VaddioRoboshotSerialBuffer buffer)
		{
			buffer.OnPasswordPrompt += BufferOnPasswordPrompt;
			buffer.OnUsernamePrompt += BufferOnUsernamePrompt;
			buffer.OnSerialTelnetHeader += BufferOnSerialTelnetHeader;
		}

		/// <summary>
		/// Unsubscribe from the buffer events.
		/// </summary>
		/// <param name="buffer"></param>
		private void Unsubscribe(VaddioRoboshotSerialBuffer buffer)
		{
			buffer.OnPasswordPrompt -= BufferOnPasswordPrompt;
			buffer.OnUsernamePrompt -= BufferOnUsernamePrompt;
			buffer.OnSerialTelnetHeader -= BufferOnSerialTelnetHeader;
		}

		/// <summary>
		/// Called when the device prompts for a username.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void BufferOnUsernamePrompt(object sender, EventArgs eventArgs)
		{
			m_ConnectionStateManager.Send((Username ?? string.Empty) + DELIMITER);
		}

		/// <summary>
		/// Called when the device prompts for a password.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void BufferOnPasswordPrompt(object sender, EventArgs eventArgs)
		{
			m_ConnectionStateManager.Send((Password ?? string.Empty) + DELIMITER);
		}

		/// <summary>
		/// Called when we receive a telnet handshake message from the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void BufferOnSerialTelnetHeader(object sender, StringEventArgs args)
		{
			TelnetCommand command = TelnetCommand.Parse(args.Data);
			m_ConnectionStateManager.Send(command.Reject().Serialize());
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
					Logger.Log(eSeverity.Error, "No serial port with Id {0}", settings.Port);
				}
			}

			SetPort(port);

			SupportedCameraFeatures = eCameraFeatures.PanTiltZoom | eCameraFeatures.Presets | eCameraFeatures.Home;
		}

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(VaddioRoboshotCameraDeviceSettings settings, IDeviceFactory factory,
		                                    Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new GenericCameraRouteSourceControl<VaddioRoboshotCameraDevice>(this, 0));
			addControl(new CameraDeviceControl(this, 1));
			addControl(new PowerDeviceControl<VaddioRoboshotCameraDevice>(this, 2));
		}

		/// <summary>
		/// Override to add actions on StartSettings
		/// This should be used to start communications with devices and perform initial actions
		/// </summary>
		protected override void StartSettingsFinal()
		{
			base.StartSettingsFinal();

			m_ConnectionStateManager.Start();
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			foreach (IConsoleNodeBase node in DeviceWithPowerConsole.GetConsoleNodes(this))
				yield return node;

			if (m_ConnectionStateManager.Port != null)
				yield return m_ConnectionStateManager.Port;
		}

		/// <summary>
		/// Wrokaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in DeviceWithPowerConsole.GetConsoleCommands(this))
				yield return command;
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
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			DeviceWithPowerConsole.BuildConsoleStatus(this, addRow);

			addRow("Pan Speed", m_PanSpeed);
			addRow("Tilt Speed", m_TiltSpeed);
			addRow("Zoom Speed", m_ZoomSpeed);
		}

		#endregion
	}
}
