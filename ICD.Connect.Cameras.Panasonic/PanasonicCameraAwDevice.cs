using ICD.Common.EventArguments;
using ICD.Common.Properties;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Cameras;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.WebPorts;
using ICD.Connect.Protocol.SerialBuffers;
using ICD.Connect.Protocol.SerialQueues;
using ICD.Connect.Settings.Core;
using Newtonsoft.Json.Linq;
using System;

namespace ICD.Connect.Cameras.Panasonic
{
    public sealed class PanasonicCameraAwDevice : AbstractCameraDevice<PanasonicCameraAwDeviceSettings>
    {

        private PanasonicCommandHandler CommandHandler;
        //Messages can only be sent once every 130 seconds
        private const long SENDING_UPDATE_INTERVAL = 130;
        private const long FAILURE_UPDATE_INTERVAL_INCREMENT = 1000 * 10;
        private const long FAILURE_UPDATE_INTERVAL_LIMIT = 1000 * 60;

        //private readonly SafeTimer m_SharingTimer;
        //private readonly SafeCriticalSection m_SharingTimerSection;

        //private long m_SharingUpdateInterval;

        private ISerialBuffer SerialBuffer { get; set; }
        private ISerialQueue SerialQueue { get; set; }

        private bool m_ClearedToSend;

        private const string PORT_ACCEPT = "application/json";
        private IWebPort m_Port;

        public override void Move(eCameraAction action)
        {
            string commandUrl = CommandHandler.Move(action);
            string response = m_Port.Get(commandUrl);

            ParsePortData(response);
        }

        public override void Stop()
        {
            string commandUrl = CommandHandler.Stop();
            string response = m_Port.Get(commandUrl);

            ParsePortData(response);
        }

        public PanasonicCameraAwDevice()
        {
            CommandHandler = new PanasonicCommandHandler();
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

            if (m_Port != null)
                m_Port.Accept = PORT_ACCEPT;
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

        #endregion

        private void ParsePortData(string response)
        {
            if (string.IsNullOrEmpty(response))
                return;

            response = response.Trim();

            try
            {
                if (string.Equals(response.Substring(0, 3), "rER", StringComparison.OrdinalIgnoreCase))
                {
                    Log(eSeverity.Error, "Port data returned error with code {0}", response);
                }
            }
            catch (Exception)
            {
                Log(eSeverity.Error, "Failed to parse data returned from camera");
            }
        }

        /// <summary>
        /// Increments the update interval up to the failure limit.
        /// </summary>
        private void IncrementUpdateInterval()
        {
            //m_SharingUpdateInterval += FAILURE_UPDATE_INTERVAL_INCREMENT;
            //if (m_SharingUpdateInterval > FAILURE_UPDATE_INTERVAL_LIMIT)
            //    m_SharingUpdateInterval = FAILURE_UPDATE_INTERVAL_LIMIT;

            //m_SharingTimer.Reset(m_SharingUpdateInterval, m_SharingUpdateInterval);
        }

    }
}