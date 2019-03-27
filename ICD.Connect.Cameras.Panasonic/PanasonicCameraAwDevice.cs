﻿using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Cameras.Panasonic
{
	public sealed class PanasonicCameraAwDevice : AbstractCameraDevice<PanasonicCameraAwDeviceSettings>,
		ICameraWithPanTilt, ICameraWithZoom, IDeviceWithPower

	{
		#region Properties
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

		private int? m_PanTiltSpeed;
		private int? m_ZoomSpeed;

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

		#region ICameraWithPanTilt
		public void PanTilt(eCameraPanTiltAction action)
		{
			SendCommand(m_PanTiltSpeed == null
							? PanasonicCommandBuilder.GetPanTiltCommand(action)
							: PanasonicCommandBuilder.GetPanTiltCommand(action, m_PanTiltSpeed.Value));
		}
		#endregion

		#region ICameraWithZoom
		public void Zoom(eCameraZoomAction action)
		{
			SendCommand(m_ZoomSpeed == null
							? PanasonicCommandBuilder.GetZoomCommand(action)
							: PanasonicCommandBuilder.GetZoomCommand(action, m_ZoomSpeed.Value));
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

			Unsubscribe(m_Port);
			m_Port = port;
			Subscribe(m_Port);

			UpdateCachedOnlineStatus();
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
					try
					{
						string response;
						m_Port.Get(command, out response);
						ParsePortData(command, response);
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

			m_PanTiltSpeed = settings.PanTiltSpeed;
			m_ZoomSpeed = settings.ZoomSpeed;
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
