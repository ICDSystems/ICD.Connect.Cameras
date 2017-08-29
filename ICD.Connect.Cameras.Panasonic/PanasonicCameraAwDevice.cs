﻿using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Cameras;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Cameras.Panasonic
{

    public sealed class PanasonicCameraAwDevice : AbstractCameraDevice<PanasonicCameraAwDeviceSettings>
    {
        #region properties

        private const long RATE_LIMIT = 130;

        private static readonly Dictionary<string, string> s_ErrorMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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

        #region Inerface Methods

        public override void Move(eCameraAction action)
        {
            SendCommand(PanasonicCommandBuilder.Move(action));
        }

        public override void Stop()
        {
            SendCommand(PanasonicCommandBuilder.Stop());
            SendCommand(PanasonicCommandBuilder.StopZoom());
        }

        #endregion

        #region methods

        public PanasonicCameraAwDevice()
        {
            m_CommandList = new Queue<string>();
            m_CommandSection = new SafeCriticalSection();

            m_DelayTimer = new IcdTimer();
            m_DelayTimer.OnElapsed += TimerElapsed;

            Controls.Add(new GenericCameraRouteSourceControl<PanasonicCameraAwDevice>(this, 0));
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
                        ParsePortData(command, m_Port.Get(command));
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
    }
}