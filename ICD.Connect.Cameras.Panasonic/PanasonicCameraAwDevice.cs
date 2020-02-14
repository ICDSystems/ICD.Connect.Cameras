using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Commands;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
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

		/// <summary>
		/// Gets the maximum number of presets this camera can support
		/// </summary>
		public override int MaxPresets { get { throw new NotSupportedException(); } }

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

			Controls.Add(new GenericCameraRouteSourceControl<PanasonicCameraAwDevice>(this, 0));
			Controls.Add(new CameraDeviceControl(this, 1));
			Controls.Add(new PowerDeviceControl<PanasonicCameraAwDevice>(this, 2));
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
		public override void SendCameraHome()
		{
			throw new NotSupportedException();
		}

		#endregion

		#region IDeviceWithPower

		public void PowerOn()
		{
			SendCommand(PanasonicCommandBuilder.GetPowerOnCommand());
		}

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
						Log(eSeverity.Error, "Failed to make request - port is null");
						return;
					}
					try
					{
						WebPortResponse response = m_Port.Get(command);
						ParsePortData(command, response.DataAsString);
					}
					catch (Exception ex)
					{
						Log(eSeverity.Error, "Failed to make request{0}{1}{0}{2}", IcdEnvironment.NewLine,
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

			response = response.Trim();

			if (command.ToLower().Contains(response.ToLower()))
				return;

			string code = response.Substring(0, 3);

			string message;
			if (!s_ErrorMap.TryGetValue(code, out message))
				message = "Unexpected error code";

			Log(eSeverity.Error, "{0} - {1}", response, message);
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
					Log(eSeverity.Error, "No Web Port with id {0}", settings.Port);
				}
			}

			SetPort(port);

			SupportedCameraFeatures = eCameraFeatures.PanTiltZoom;

			m_PanTiltSpeed = settings.PanTiltSpeed;
			m_ZoomSpeed = settings.ZoomSpeed;
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
