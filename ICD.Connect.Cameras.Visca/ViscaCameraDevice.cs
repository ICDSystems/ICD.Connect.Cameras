using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
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
using ICD.Connect.Protocol.EventArguments;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Protocol.SerialBuffers;
using ICD.Connect.Protocol.SerialQueues;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Settings;

namespace ICD.Connect.Cameras.Visca
{
	public sealed class ViscaCameraDevice : AbstractCameraDevice<ViscaCameraDeviceSettings>, IDeviceWithPower
	{
		private const int MAX_RETRY_ATTEMPTS = 20;
		private const int DEFAULT_ID = 1;
		private const char DELIMITER = '\xFF';

		/// <summary>
		/// Raised when the powered state changes.
		/// </summary>
		public event EventHandler<PowerDeviceControlPowerStateApiEventArgs> OnPowerStateChanged;

		#region Private Members

		private readonly Dictionary<string, int> m_RetryCounts = new Dictionary<string, int>();
		private readonly SafeCriticalSection m_RetryLock = new SafeCriticalSection();
		private readonly ComSpecProperties m_ComSpecProperties;
		private readonly ConnectionStateManager m_ConnectionStateManager;

		private int? m_PanTiltSpeed;
		private int? m_ZoomSpeed;
		private ePowerState m_PowerState;
		private ISerialQueue m_SerialQueue;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the maximum number of presets this camera can support
		/// </summary>
		public override int MaxPresets { get { return 0; } }

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
		public ViscaCameraDevice()
		{
			m_ComSpecProperties = new ComSpecProperties();

			m_ConnectionStateManager = new ConnectionStateManager(this) { ConfigurePort = ConfigurePort };
			m_ConnectionStateManager.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;

			SupportedCameraFeatures = eCameraFeatures.PanTiltZoom;
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
		}

		/// <summary>
		/// Called when the port online state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PortOnIsOnlineStateChanged(object sender, BoolEventArgs e)
		{
			UpdateCachedOnlineStatus();
		}

		#region PTZ

		/// <summary>
		/// Begins panning the camera
		/// </summary>
		/// <param name="action"></param>
		public override void Pan(eCameraPanAction action)
		{
			SendCommand(m_PanTiltSpeed == null
							? ViscaCommand.GetPanCommand(DEFAULT_ID, action)
							: ViscaCommand.GetPanCommand(DEFAULT_ID, action, m_PanTiltSpeed.Value));
		}

		/// <summary>
		/// Begin tilting the camera.
		/// </summary>
		public override void Tilt(eCameraTiltAction action)
		{
			SendCommand(m_PanTiltSpeed == null
							? ViscaCommand.GetTiltCommand(DEFAULT_ID, action)
							: ViscaCommand.GetTiltCommand(DEFAULT_ID, action, m_PanTiltSpeed.Value));
		}

		/// <summary>
		/// Begin zooming the camera
		/// </summary>
		/// <param name="action"></param>
		public override void Zoom(eCameraZoomAction action)
		{
			SendCommand(m_ZoomSpeed == null
							? ViscaCommand.GetZoomCommand(DEFAULT_ID, action)
							: ViscaCommand.GetZoomCommand(DEFAULT_ID, action, m_ZoomSpeed.Value));
		}

		/// <summary>
		/// Gets the stored camera presets.
		/// </summary>
		public override IEnumerable<CameraPreset> GetPresets()
		{
			return Enumerable.Empty<CameraPreset>();
		}

		/// <summary>
		/// Tells the camera to change its position to the given preset.
		/// </summary>
		/// <param name="presetId">The id of the preset to position to.</param>
		public override void ActivatePreset(int presetId)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Stores the cameras current position in the given preset index.
		/// </summary>
		/// <param name="presetId">The index to store the preset at.</param>
		public override void StorePreset(int presetId)
		{
			throw new NotSupportedException();
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
			throw new NotSupportedException();
		}

		/// <summary>
		/// Stores the current position as the home position.
		/// </summary>
		public override void StoreHome()
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sets the wrapped port for communication with the hardware.
		/// </summary>
		/// <param name="port"></param>
		public void SetPort(ISerialPort port)
		{
			m_ConnectionStateManager.SetPort(port, false);

			// todo: Initialize when port comes online?
			if (port != null && port.IsOnline)
			{
				SendCommand(ViscaCommand.GetSetAddressCommand());
				SendCommand(ViscaCommand.GetClearCommand());
				SendCommand(ViscaCommand.GetPowerInquiryCommand(DEFAULT_ID));
			}
		}

		/// <summary>
		/// Configures the given port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		private void ConfigurePort(IPort port)
		{
			// Com
			if (port is IComPort)
				(port as IComPort).ApplyDeviceConfiguration(m_ComSpecProperties);

			ISerialBuffer buffer = new DelimiterSerialBuffer(DELIMITER);
			SerialQueue queue = new SerialQueue();
			queue.SetPort(port as ISerialPort);
			queue.SetBuffer(buffer);
			queue.Timeout = 3 * 1000;

			SetSerialQueue(queue);

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_ConnectionStateManager != null && m_ConnectionStateManager.IsOnline;
		}

		/// <summary>
		/// Powers the device on.
		/// </summary>
		public void PowerOn()
		{
			SendCommand(ViscaCommand.GetPowerOnCommand(DEFAULT_ID));
		}

		/// <summary>
		/// Powers the device off.
		/// </summary>
		public void PowerOff()
		{
			SendCommand(ViscaCommand.GetPowerOffCommand(DEFAULT_ID));
		}

		/// <summary>
		/// Queues the command to be sent to the device.
		/// </summary>
		/// <param name="command"></param>
		private void SendCommand(ViscaCommand command)
		{
			m_SerialQueue.Enqueue(command);
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
		/// Called when a complete response is received from the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SerialQueueOnSerialResponse(object sender, SerialResponseEventArgs args)
		{
			if (args.Data == null)
				return;

			eViscaResponse code = ViscaResponseUtils.ToResponse(args.Response);

			if (code.IsError())
				HandleError((ViscaCommand)args.Data, code);
			else
				HandleSuccess((ViscaCommand)args.Data, args.Response, code);
		}

		/// <summary>
		/// Handles good responses from the device.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <param name="code"></param>
		private void HandleSuccess(ViscaCommand request, string response, eViscaResponse code)
		{
			// Update power state feedback
			if (request.CommandEquals(ViscaCommand.GetPowerOnCommand(DEFAULT_ID)))
				PowerState = ePowerState.PowerOn;
			else if (request.CommandEquals(ViscaCommand.GetPowerOffCommand(DEFAULT_ID)))
				PowerState = ePowerState.PowerOff;
			else if (request.CommandEquals(ViscaCommand.GetPowerInquiryCommand(DEFAULT_ID)))
				PowerState = ViscaResponseUtils.GetSingleValue(response) == 2 ? ePowerState.PowerOn : ePowerState.PowerOff;
		}

		/// <summary>
		/// Handles error responses from the device.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="response"></param>
		private void HandleError(ViscaCommand data, eViscaResponse response)
		{
			Logger.Log(eSeverity.Error, "Got error response for command {0} - {1}",
			           StringUtils.ToHexLiteral(data.Serialize()), response.ToString());

			RetryCommand(data);
		}

		/// <summary>
		/// Increments the retry count for the command and sends again if the retry attempt limit hasn't been reached.
		/// </summary>
		/// <param name="command"></param>
		private void RetryCommand(ViscaCommand command)
		{
			IncrementRetryCount(command);

			if (GetRetryCount(command) <= MAX_RETRY_ATTEMPTS)
			{
				m_SerialQueue.EnqueuePriority(command);
				return;
			}

			Logger.Log(eSeverity.Error, "Command {0} failed too many times and hit the retry limit.",
			           StringUtils.ToMixedReadableHexLiteral(command.Serialize()));
			ResetRetryCount(command);
		}

		/// <summary>
		/// Increments the retry count for the given command.
		/// </summary>
		/// <param name="command"></param>
		private void IncrementRetryCount(ViscaCommand command)
		{
			string serial = command.Serialize();
			m_RetryLock.Execute(() => m_RetryCounts[serial] = m_RetryCounts.GetDefault(serial) + 1);
		}

		/// <summary>
		/// Gets the retry count for the given command.
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		private int GetRetryCount(ViscaCommand command)
		{
			return m_RetryLock.Execute(() => m_RetryCounts.GetDefault(command.Serialize()));
		}

		/// <summary>
		/// Resets the retry count for the given command.
		/// </summary>
		/// <param name="command"></param>
		private void ResetRetryCount(ViscaCommand command)
		{
			m_RetryLock.Execute(() => m_RetryCounts.Remove(command.Serialize()));
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(ViscaCameraDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Port = m_ConnectionStateManager.PortNumber;
			settings.PanTiltSpeed = m_PanTiltSpeed;
			settings.ZoomSpeed = m_ZoomSpeed;

			settings.Copy(m_ComSpecProperties);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_ComSpecProperties.ClearComSpecProperties();

			SetPort(null);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(ViscaCameraDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_ComSpecProperties.Copy(settings);

			m_PanTiltSpeed = settings.PanTiltSpeed;
			m_ZoomSpeed = settings.ZoomSpeed;

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
		}

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(ViscaCameraDeviceSettings settings, IDeviceFactory factory,
		                                    Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new GenericCameraRouteSourceControl<ViscaCameraDevice>(this, 0));
			addControl(new CameraDeviceControl(this, 1));
			addControl(new PowerDeviceControl(this, 2));
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
		/// Workaround for "unverifiable code" warning.
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

			addRow("Pan/Tilt Speed", m_PanTiltSpeed);
			addRow("Zoom Speed", m_ZoomSpeed);
		}

		#endregion
	}
}
