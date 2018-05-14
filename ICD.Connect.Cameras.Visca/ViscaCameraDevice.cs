﻿using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Cameras.Controls;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Protocol.Data;
using ICD.Connect.Protocol.EventArguments;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Protocol.SerialBuffers;
using ICD.Connect.Protocol.SerialQueues;
using ICD.Connect.Settings;

namespace ICD.Connect.Cameras.Visca
{
	public sealed class ViscaCameraDevice : AbstractCameraDevice<ViscaCameraDeviceSettings>,
		ICameraWithPanTilt, ICameraWithZoom, IDeviceWithPower
	{
		private ISerialQueue SerialQueue { get; set; }

		private readonly Dictionary<string, int> m_RetryCounts = new Dictionary<string, int>();
		private readonly SafeCriticalSection m_RetryLock = new SafeCriticalSection();

		private const int MAX_RETRY_ATTEMPTS = 20;
		private const int DEFAULT_ID = 1;
		private const char DELIMITER = '\xFF';

		private int? m_PanTiltSpeed;
		private int? m_ZoomSpeed;

		public ViscaCameraDevice()
		{
			Controls.Add(new GenericCameraRouteSourceControl<ViscaCameraDevice>(this, 0));
			Controls.Add(new PanTiltControl<ViscaCameraDevice>(this, 1));
			Controls.Add(new ZoomControl<ViscaCameraDevice>(this, 2));
			Controls.Add(new PowerDeviceControl<ViscaCameraDevice>(this, 3));
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
			base.DisposeFinal(disposing);
		}

		/// <summary>
		/// Sets the wrapped port for communication with the hardware.
		/// </summary>
		/// <param name="port"></param>
		private void SetPort(ISerialPort port)
		{
			if (port == SerialQueue.Port)
				return;

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
			return SerialQueue != null && SerialQueue.Port != null && SerialQueue.Port.IsOnline;
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

		#endregion

		#region SerialQueue Callbacks

		/// <summary>
		/// Subscribe to the serial queue events.
		/// </summary>
		/// <param name="queue"></param>
		private void Subscribe(ISerialQueue queue)
		{
			if (queue == null)
				return;

			queue.OnSerialResponse += SerialQueueOnSerialResponse;
			queue.OnTimeout += SerialQueueOnSerialTimeout;

			if (queue.Port == null)
				return;

			queue.Port.OnIsOnlineStateChanged += SerialQueueOnIsOnlineStateChanged;
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
			queue.OnTimeout -= SerialQueueOnSerialTimeout;

			if (queue.Port == null)
				return;

			queue.Port.OnIsOnlineStateChanged -= SerialQueueOnIsOnlineStateChanged;
		}

		private void SerialQueueOnSerialResponse(object sender, SerialResponseEventArgs args)
		{
			if (args.Data == null)
				return;

			IcdConsole.PrintLine(String.Format("Serial Data {0} - Serial Response {1}",
											   StringUtils.ToHexLiteral(args.Data.Serialize()),
											   StringUtils.ToHexLiteral(args.Response)));

			if (ViscaResponseHandler.HandleResponse(args.Response) == eViscaResponse.OK)
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
			IcdConsole.PrintLine(response);
		}

		private void ParseError(ISerialData data)
		{
			RetryCommand(data.Serialize());
		}

		private void RetryCommand(String command)
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

		private void IncrementRetryCount(String command)
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

		private void SerialQueueOnSerialTimeout(object sender, SerialDataEventArgs args)
		{
		}

		private void SerialQueueOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs args)
		{
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

			if (SerialQueue != null && SerialQueue.Port != null)
				settings.Port = SerialQueue.Port.Id;
			else
				settings.Port = null;
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
		protected override void ApplySettingsFinal(ViscaCameraDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			ISerialPort port = null;
			if (settings.Port != null)
				port = factory.GetPortById((int)settings.Port) as ISerialPort;

			if (port == null)
				Log(eSeverity.Error, String.Format("No serial port with Id {0}", settings.Port));

			SetPort(port);
			if (port != null && port.IsOnline)
			{
				SendCommand(ViscaCommandBuilder.GetSetAddressCommand());
				SendCommand(ViscaCommandBuilder.GetClearCommand());
			}

			m_PanTiltSpeed = settings.PanTiltSpeed;
			m_ZoomSpeed = settings.ZoomSpeed;
		}

		#endregion

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
			where TData : ISerialData
		{
			SerialQueue.Enqueue(command, comparer);
		}

		[PublicAPI]
		public void SendCommand(string command)
		{
			SendCommand(new SerialData(command));
		}

		private void SetSerialQueue(ISerialQueue serialQueue)
		{
			Unsubscribe(SerialQueue);

			if (SerialQueue != null)
				SerialQueue.Dispose();

			SerialQueue = serialQueue;

			Subscribe(SerialQueue);

			UpdateCachedOnlineStatus();
		}
	}
}
