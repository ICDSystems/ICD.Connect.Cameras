﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using ICD.Connect.Protocol;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Cameras.Vaddio
{
	public sealed class VaddioRoboshotCameraDevice : AbstractCameraDevice<VaddioRoboshotCameraDeviceSettings>,
		ICameraWithPanTilt, ICameraWithZoom, ICameraWithPresets, IDeviceWithPower
	{
		/// <summary>
		/// Raised when the presets are changed.
		/// </summary>
		public event EventHandler OnPresetsChanged;

		private const char DELIMITER = '\r';

		private const int DEFAULT_PAN_SPEED = 12;
		private const int DEFAULT_TILT_SPEED = 10;
		private const int DEFAULT_ZOOM_SPEED = 3;

		private const string DEFAULT_USERNAME = "admin";
		private const string DEFAULT_PASSWORD = "password";

		private readonly ConnectionStateManager m_ConnectionStateManager;
		private readonly VaddioRoboshotSerialBuffer m_SerialBuffer;

		private int m_PanSpeed;
		private int m_TiltSpeed;
		private int m_ZoomSpeed;

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
		public int MaxPresets { get { return 16; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public VaddioRoboshotCameraDevice()
		{
			m_SerialBuffer = new VaddioRoboshotSerialBuffer();
			Subscribe(m_SerialBuffer);

			m_ConnectionStateManager = new ConnectionStateManager(this);
			m_ConnectionStateManager.OnSerialDataReceived += PortOnSerialDataReceived;
			m_ConnectionStateManager.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
			m_ConnectionStateManager.OnConnectedStateChanged += PortOnConnectedStateChanged;

			Controls.Add(new GenericCameraRouteSourceControl<VaddioRoboshotCameraDevice>(this, 0));
			Controls.Add(new PanTiltControl<VaddioRoboshotCameraDevice>(this, 1));
			Controls.Add(new ZoomControl<VaddioRoboshotCameraDevice>(this, 2));
			Controls.Add(new PresetControl<VaddioRoboshotCameraDevice>(this, 3));
			Controls.Add(new PowerDeviceControl<VaddioRoboshotCameraDevice>(this, 4));
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnPresetsChanged = null;

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
		/// Gets the stored camera presets.
		/// </summary>
		public IEnumerable<CameraPreset> GetPresets()
		{
			return Enumerable.Range(1, 16).Select(i => new CameraPreset(i, string.Format("Preset {0}", i)));
		}

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		public void ActivatePreset(int presetId)
		{
			string command = string.Format("camera preset recall {0}", presetId);
			SendCommand(command);
		}

		/// <summary>
		/// Stores the cameras current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		public void StorePreset(int presetId)
		{
			string command = string.Format("camera preset store {0}", presetId);
			SendCommand(command);
		}

		/// <summary>
		/// Starts rotating the camera with the given action.
		/// </summary>
		/// <param name="action"></param>
		public void PanTilt(eCameraPanTiltAction action)
		{
			string command;

			switch (action)
			{
				case eCameraPanTiltAction.Left:
					command = string.Format("camera pan left {0}", m_PanSpeed);
					break;
				case eCameraPanTiltAction.Right:
					command = string.Format("camera pan right {0}", m_PanSpeed);
					break;
				case eCameraPanTiltAction.Up:
					command = string.Format("camera tilt up {0}", m_TiltSpeed);
					break;
				case eCameraPanTiltAction.Down:
					command = string.Format("camera tilt down {0}", m_TiltSpeed);
					break;

				case eCameraPanTiltAction.Stop:
					SendCommand("camera pan stop");
					SendCommand("camera tilt stop");
					return;

				default:
					throw new ArgumentOutOfRangeException("action");
			}

			SendCommand(command);
		}

		/// <summary>
		/// Starts zooming the camera with the given action.
		/// </summary>
		/// <param name="action"></param>
		public void Zoom(eCameraZoomAction action)
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
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			CameraWithPanTiltConsole.BuildConsoleStatus(this, addRow);
			CameraWithZoomConsole.BuildConsoleStatus(this, addRow);
			CameraWithPresetsConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in CameraWithPanTiltConsole.GetConsoleCommands(this))
				yield return command;

			foreach (IConsoleCommand command in CameraWithZoomConsole.GetConsoleCommands(this))
				yield return command;

			foreach (IConsoleCommand command in CameraWithPresetsConsole.GetConsoleCommands(this))
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

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			foreach (IConsoleNodeBase node in CameraWithPanTiltConsole.GetConsoleNodes(this))
				yield return node;

			foreach (IConsoleNodeBase node in CameraWithZoomConsole.GetConsoleNodes(this))
				yield return node;

			foreach (IConsoleNodeBase node in CameraWithPresetsConsole.GetConsoleNodes(this))
				yield return node;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		#endregion
	}
}