using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Commands;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Cameras.Panasonic
{
	public sealed class PanasonicCameraAwDevice : AbstractCameraDevice<PanasonicCameraAwDeviceSettings>,
		ICameraWithPanTilt, ICameraWithZoom, IDeviceWithPower

	{
		#region Properties
		public override int? CameraId { get { return 0; } }//TODO: Fix Me
		private const long RATE_LIMIT = 130;

		private static readonly Dictionary<string, string> s_ErrorMap =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				{"rER", "Port data returned error with code"},
				{"er1", "Command is unsupported"},
				{"er2", "Camera is powered down or busy"},
				{"er3", "Command is out of range"},
			};

		private IWebPort m_Port;

		private readonly IcdTimer m_DelayTimer;

		private readonly SafeCriticalSection m_CommandSection;
		private readonly Queue<string> m_CommandList;

		#endregion

		public PanasonicCameraAwDevice()
		{
			m_CommandList = new Queue<string>();
			m_CommandSection = new SafeCriticalSection();

			m_DelayTimer = new IcdTimer();
			m_DelayTimer.OnElapsed += TimerElapsed;

			Controls.Add(new GenericCameraRouteSourceControl<PanasonicCameraAwDevice>(this, 0));
			Controls.Add(new PanTiltControl<PanasonicCameraAwDevice>(this, 1));
			Controls.Add(new ZoomControl<PanasonicCameraAwDevice>(this, 2));
			Controls.Add(new PowerDeviceControl<PanasonicCameraAwDevice>(this, 3));
		}

		#region Methods

		/// <summary>
		/// Sets the port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		[PublicAPI]
		public void SetPort(IWebPort port)
		{
			if (port == m_Port)
				return;

			Unsubscribe(m_Port);
			m_Port = port;
			Subscribe(m_Port);

			UpdateCachedOnlineStatus();
		}

		public void PowerOn()
		{
			SendCommand(PanasonicCommandBuilder.PowerOn());
		}

		public void PowerOff()
		{
			SendCommand(PanasonicCommandBuilder.PowerOff());
		}

		public void PanTilt(eCameraPanTiltAction action)
		{
			SendCommand(PanasonicCommandBuilder.PanTilt(action));
		}

		public void Zoom(eCameraZoomAction action)
		{
			SendCommand(PanasonicCommandBuilder.Zoom(action));
		}

		#endregion

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

		/// <summary>
		/// Responsible for dispatching commands from the command buffer.
		/// </summary>
		/// <param name="command"></param>
		private void SendCommand(string command)
		{
			m_CommandSection.Enter();

			try
			{
				if (m_DelayTimer.IsRunning)
					m_CommandList.Enqueue(command);
				else
				{
					try
					{
						string response;
						m_Port.Get(command, out response);
						ParsePortData(command, response );
					}
					catch (Exception ex)
					{
						Log(eSeverity.Error, "Failed to make request - {0}", ex.Message);
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

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Port != null && m_Port.IsOnline;
		}

		/// <summary>
		/// Logs to logging core.
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		private void Log(eSeverity severity, string message, params object[] args)
		{
			message = string.Format(message, args);
			message = string.Format("{0} - {1}", GetType().Name, message);

			ServiceProvider.GetService<ILoggerService>().AddEntry(severity, message);
		}

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
		/// <param name="boolEventArgs"></param>
		private void PortOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
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
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetPort(null);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(PanasonicCameraAwDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			IWebPort port = null;

			if (settings.Port != null)
				port = factory.GetPortById((int)settings.Port) as IWebPort;

			if (port == null)
				Log(eSeverity.Error, "No Web Port with id {0}", settings.Port);

			SetPort(port);
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
