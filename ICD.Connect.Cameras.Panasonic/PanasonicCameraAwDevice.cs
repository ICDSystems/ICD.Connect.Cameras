using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Controls.Power;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Settings;

namespace ICD.Connect.Cameras.Panasonic
{
	public sealed class PanasonicCameraAwDevice : AbstractCameraDevice<PanasonicCameraAwDeviceSettings>, IDeviceWithPower
	{
		private const long RATE_LIMIT = 130;

		/// <summary>
		/// Raised when the powered state changes.
		/// </summary>
		public event EventHandler<PowerDeviceControlPowerStateApiEventArgs> OnPowerStateChanged;

		private static readonly Dictionary<string, string> s_ErrorMap =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				{"rER", "Port data returned error with code"},
				{"er1", "Command is unsupported"},
				{"er2", "Camera is powered down or busy"},
				{"er3", "Command is out of range"},
			};

		private readonly IcdTimer m_DelayTimer;
		private readonly SafeCriticalSection m_CommandSection;
		private readonly Queue<string> m_CommandList;

		private readonly UriProperties m_UriProperties;
		private readonly WebProxyProperties m_WebProxyProperties;

		private IWebPort m_Port;
		private int? m_PanTiltSpeed;
		private int? m_ZoomSpeed;
		private ePowerState m_PowerState;

		#region Properties

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
		public PanasonicCameraAwDevice()
		{
			m_UriProperties = new UriProperties();
			m_WebProxyProperties = new WebProxyProperties();

			m_CommandList = new Queue<string>();
			m_CommandSection = new SafeCriticalSection();

			m_DelayTimer = new IcdTimer();
			m_DelayTimer.OnElapsed += TimerElapsed;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnPowerStateChanged = null;

			base.DisposeFinal(disposing);
		}

		#region Camera Control Methods

		public override void Pan(eCameraPanAction action)
		{
			SendCommand(m_PanTiltSpeed == null
							? PanasonicCommandBuilder.GetPanCommand(action)
							: PanasonicCommandBuilder.GetPanCommand(action, m_PanTiltSpeed.Value));
		}

		public override void Tilt(eCameraTiltAction action)
		{
			SendCommand(m_PanTiltSpeed == null
							? PanasonicCommandBuilder.GetTiltCommand(action)
							: PanasonicCommandBuilder.GetTiltCommand(action, m_PanTiltSpeed.Value));
		}
		
		public override void Zoom(eCameraZoomAction action)
		{
			SendCommand(m_ZoomSpeed == null
				            ? PanasonicCommandBuilder.GetZoomCommand(action)
				            : PanasonicCommandBuilder.GetZoomCommand(action, m_ZoomSpeed.Value));
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

		#region IDeviceWithPower

		/// <summary>
		/// Powers on the device.
		/// </summary>
		public void PowerOn()
		{
			SendCommand(PanasonicCommandBuilder.GetPowerOnCommand());
		}

		/// <summary>
		/// Powers off the device.
		/// </summary>
		public void PowerOff()
		{
			SendCommand(PanasonicCommandBuilder.GetPowerOffCommand());
		}

		#endregion

		#region DeviceBase

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Port != null && m_Port.IsOnline;
		}

		#endregion

		#region Public API

		/// <summary>
		/// Sets the port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		[PublicAPI]
		public void SetPort(IWebPort port)
		{
			if (port == m_Port)
				return;

			ConfigurePort(port);

			Unsubscribe(m_Port);
			m_Port = port;
			Subscribe(m_Port);

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Configures the given port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		private void ConfigurePort(IWebPort port)
		{
			// Web
			if (port != null)
			{
				port.ApplyDeviceConfiguration(m_UriProperties);
				port.ApplyDeviceConfiguration(m_WebProxyProperties);
			}
		}

		#endregion

		#region Private Methods

		private void TimerElapsed(object sender, EventArgs args)
		{
			m_CommandSection.Enter();

			try
			{
				m_DelayTimer.Stop();

				if (m_CommandList.Count > 0)
					SendCommand(m_CommandList.Dequeue());

				m_DelayTimer.Restart(RATE_LIMIT);
			}
			finally
			{
				m_CommandSection.Leave();
			}
		}

		private void SendCommand(string command)
		{
			m_CommandSection.Enter();

			try
			{
				if (m_DelayTimer.IsRunning)
					m_CommandList.Enqueue(command);
				else
				{
					if (m_Port == null)
					{
						Logger.Log(eSeverity.Error, "Failed to make request - port is null");
						return;
					}
					try
					{
						WebPortResponse response = m_Port.Get(command, null);
						ParsePortData(command, response.DataAsString);
					}
					catch (Exception ex)
					{
						Logger.Log(eSeverity.Error, "Failed to make request{0}{1}{0}{2}", IcdEnvironment.NewLine,
			                                         ex.Message, ex.StackTrace);
						m_CommandList.Clear();

						m_DelayTimer.Stop();
					}
				}

				m_DelayTimer.Restart(RATE_LIMIT);
			}
			finally
			{
				m_CommandSection.Leave();
			}
		}

		private void ParsePortData(string command, string response)
		{
			if (string.IsNullOrEmpty(response))
				return;

			response = response.Trim().ToLower();
			
			// Handle error
			string message;
			string code = response.Substring(0, Math.Min(3, response.Length));
			if (s_ErrorMap.TryGetValue(code, out message))
			{
				Logger.Log(eSeverity.Error, "{0} - {1}", response, message);
				return;
			}

			// Handle feedback
			switch (response)
			{
				case "p0":
					PowerState = ePowerState.PowerOff;
					break;

				case "p1":
					PowerState = ePowerState.PowerOn;
					break;
			}
		}

		#endregion

		#region Port Callbacks

		/// <summary>
		/// Subscribe to the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(IWebPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(IWebPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged -= PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Called when the port online state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PortOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(PanasonicCameraAwDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Port = m_Port == null ? (int?)null : m_Port.Id;
			settings.PanTiltSpeed = m_PanTiltSpeed;
			settings.ZoomSpeed = m_ZoomSpeed;

			settings.Copy(m_UriProperties);
			settings.Copy(m_WebProxyProperties);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SupportedCameraFeatures = eCameraFeatures.None;

			SetPort(null);

			m_UriProperties.ClearUriProperties();
			m_WebProxyProperties.ClearProxyProperties();
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(PanasonicCameraAwDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_UriProperties.Copy(settings);
			m_WebProxyProperties.Copy(settings);

			IWebPort port = null;

			if (settings.Port != null)
			{
				try
				{
					port = factory.GetPortById((int)settings.Port) as IWebPort;
				}
				catch (KeyNotFoundException)
				{
					Logger.Log(eSeverity.Error, "No Web Port with id {0}", settings.Port);
				}
			}

			SetPort(port);

			SupportedCameraFeatures = eCameraFeatures.PanTiltZoom;

			m_PanTiltSpeed = settings.PanTiltSpeed;
			m_ZoomSpeed = settings.ZoomSpeed;
		}

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(PanasonicCameraAwDeviceSettings settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new GenericCameraRouteSourceControl<PanasonicCameraAwDevice>(this, 0));
			addControl(new CameraDeviceControl(this, 1));
			addControl(new PowerDeviceControl(this, 2));
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

			addRow("Pan/Tilt Speed", m_PanTiltSpeed);
			addRow("Zoom Speed", m_ZoomSpeed);
		}

		#endregion
	}
}
