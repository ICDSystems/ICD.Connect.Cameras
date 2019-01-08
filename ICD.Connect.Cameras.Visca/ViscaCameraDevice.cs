﻿using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Protocol;
using ICD.Connect.Protocol.Data;
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
	public sealed class ViscaCameraDevice : AbstractCameraDevice<ViscaCameraDeviceSettings>,
		ICameraWithPanTilt, ICameraWithZoom, IDeviceWithPower
	{
		private const int MAX_RETRY_ATTEMPTS = 20;
		private const int DEFAULT_ID = 1;
		private const char DELIMITER = '\xFF';

		private readonly Dictionary<string, int> m_RetryCounts = new Dictionary<string, int>();
		private readonly SafeCriticalSection m_RetryLock = new SafeCriticalSection();

		private readonly ComSpecProperties m_ComSpecProperties;

		private readonly ConnectionStateManager m_ConnectionStateManager;

		private int? m_PanTiltSpeed;
		private int? m_ZoomSpeed;
		private ISerialQueue SerialQueue { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public ViscaCameraDevice()
		{
			m_ComSpecProperties = new ComSpecProperties();

			m_ConnectionStateManager = new ConnectionStateManager(this) { ConfigurePort = ConfigurePort };
			m_ConnectionStateManager.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;

			Controls.Add(new GenericCameraRouteSourceControl<ViscaCameraDevice>(this, 0));
			Controls.Add(new PanTiltControl<ViscaCameraDevice>(this, 1));
			Controls.Add(new ZoomControl<ViscaCameraDevice>(this, 2));
			Controls.Add(new PowerDeviceControl<ViscaCameraDevice>(this, 3));
		}

		private void PortOnIsOnlineStateChanged(object sender, BoolEventArgs e)
		{
			UpdateCachedOnlineStatus();
		}

		#region PTZ

		public void PanTilt(eCameraPanTiltAction action)
		{
			SendCommand(m_PanTiltSpeed == null
							? ViscaCommandBuilder.GetPanTiltCommand(DEFAULT_ID, action)
							: ViscaCommandBuilder.GetPanTiltCommand(DEFAULT_ID, action, m_PanTiltSpeed.Value, m_PanTiltSpeed.Value));
		}

		public void Zoom(eCameraZoomAction action)
		{
			SendCommand(m_ZoomSpeed == null
							? ViscaCommandBuilder.GetZoomCommand(DEFAULT_ID, action)
							: ViscaCommandBuilder.GetZoomCommand(DEFAULT_ID, action, m_ZoomSpeed.Value));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			SetPort(null);

			m_ConnectionStateManager.OnIsOnlineStateChanged -= PortOnIsOnlineStateChanged;
			m_ConnectionStateManager.Dispose();

			base.DisposeFinal(disposing);
		}

		/// <summary>
		/// Sets the wrapped port for communication with the hardware.
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
			// Com
			if (port is IComPort)
				(port as IComPort).ApplyDeviceConfiguration(m_ComSpecProperties);

			ISerialBuffer buffer = new DelimiterSerialBuffer(DELIMITER);
			SerialQueue queue = new SerialQueue();
			queue.SetPort(port);
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
			SendCommand(ViscaCommandBuilder.GetPowerOnCommand(DEFAULT_ID));
		}

		/// <summary>
		/// Powers the device off.
		/// </summary>
		public void PowerOff()
		{
			SendCommand(ViscaCommandBuilder.GetPowerOffCommand(DEFAULT_ID));
		}

		/// <summary>
		/// Queues the command to be sent to the device.
		/// </summary>
		/// <param name="command"></param>
		[PublicAPI]
		public void SendCommand(ISerialData command)
		{
			SerialQueue.Enqueue(command);
		}

		/// <summary>
		/// Queues the command to be sent to the device.
		/// Replaces an existing command if it matches the comparer.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="comparer"></param>
		[PublicAPI]
		public void SendCommand<TData>(TData command, Func<TData, TData, bool> comparer)
			where TData : class, ISerialData
		{
			SerialQueue.Enqueue(command, comparer);
		}

		[PublicAPI]
		public void SendCommand(string command)
		{
			SendCommand(new SerialData(command));
		}

		#endregion

		#region SerialQueue Callbacks

		/// <summary>
		/// Subscribe to the serial queue events.
		/// </summary>
		/// <param name="serialQueue"></param>
		private void SetSerialQueue(ISerialQueue serialQueue)
		{
			Unsubscribe(SerialQueue);

			if (SerialQueue != null)
				SerialQueue.Dispose();

			SerialQueue = serialQueue;

			Subscribe(SerialQueue);

			UpdateCachedOnlineStatus();
		}

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

		private void SerialQueueOnSerialResponse(object sender, SerialResponseEventArgs args)
		{
			if (args.Data == null)
				return;

			eViscaResponse response = ViscaResponseHandler.HandleResponse(args.Response);
			if (!ViscaResponseHandler.ResponseIsError(response))
				ParseQuery(args.Response);
			else
			{
				Log(eSeverity.Error, ViscaResponseHandler.HandleResponse(args.Response).ToString(), args);
				ParseError(args.Data);
			}
		}

		private void ParseQuery(string data)
		{
			string response = data;
			Log(eSeverity.Debug, response);
		}

		private void ParseError(ISerialData data)
		{
			RetryCommand(data.Serialize());
		}

		private void RetryCommand(string command)
		{
			IncrementRetryCount(command);
			if (GetRetryCount(command) <= MAX_RETRY_ATTEMPTS)
				SerialQueue.EnqueuePriority(new SerialData(command));
			else
			{
				Log(eSeverity.Error, "Command {0} failed too many times and hit the retry limit.",
					StringUtils.ToMixedReadableHexLiteral(command));
				ResetRetryCount(command);
			}
		}

		private void IncrementRetryCount(string command)
		{
			m_RetryLock.Enter();

			try
			{
				if (m_RetryCounts.ContainsKey(command))
					m_RetryCounts[command]++;
				else
					m_RetryCounts.Add(command, 1);
			}
			finally
			{
				m_RetryLock.Leave();
			}
		}

		private int GetRetryCount(string command)
		{
			m_RetryLock.Enter();

			try
			{
				return m_RetryCounts.ContainsKey(command) ? m_RetryCounts[command] : 0;
			}
			finally
			{
				m_RetryLock.Leave();
			}
		}

		private void ResetRetryCount(string command)
		{
			m_RetryLock.Enter();

			try
			{
				m_RetryCounts.Remove(command);
			}
			finally
			{
				m_RetryLock.Leave();
			}
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
					Log(eSeverity.Error, "No serial port with Id {0}", settings.Port);
				}
			}

			SetPort(port);

			if (port != null && port.IsOnline)
			{
				SendCommand(ViscaCommandBuilder.GetSetAddressCommand());
				SendCommand(ViscaCommandBuilder.GetClearCommand());
			}
		}

		#endregion
	}
}
