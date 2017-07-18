using ICD.Connect.Conferencing.Cameras;
using ICD.Connect.Protocol.Ports;

namespace ICD.Connect.Cameras.Visca
{
    public sealed class ViscaCameraDevice : AbstractCameraDevice<ViscaCameraDeviceSettings>
    {
        public void Move(eCameraAction action)
        {
            
        }
        /// <summary>
        /// Sets the wrapped port for communication with the hardware.
        /// </summary>
        /// <param name="port"></param>
        public void SetPort(ISerialPort port)
        {
            if (port == m_Port)
                return;

            Unsubscribe(m_Port);
            m_Port = port;
            Subscribe(m_Port);
        }

        /// <summary>
        /// Gets the current online status of the device.
        /// </summary>
        /// <returns></returns>
        protected override bool GetIsOnlineStatus()
        {
            return m_Port != null && m_Port.IsOnline;
        }

        #endregion

        #region Port Callbacks

        /// <summary>
        /// Subscribe to the port events.
        /// </summary>
        /// <param name="port"></param>
        private void Subscribe(ISerialPort port)
        {
            if (port == null)
                return;

            port.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
        }

        /// <summary>
        /// Unsubscribe from the port events.
        /// </summary>
        /// <param name="port"></param>
        private void Unsubscribe(ISerialPort port)
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
        private void PortOnIsOnlineStateChanged(object sender, BoolEventArgs args)
        {
            UpdateCachedOnlineStatus();
        }

        #endregion

        #region Settings

        /// <summary>
        /// Override to apply properties to the settings instance.
        /// </summary>
        /// <param name="settings"></param>
        protected override void CopySettingsFinal(TSettings settings)
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
        protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
        {
            base.ApplySettingsFinal(settings, factory);

            ISerialPort port = null;

            if (settings.Port != null)
            {
                port = factory.GetPortById((int)settings.Port) as ISerialPort;
                if (port == null)
                    Logger.AddEntry(eSeverity.Error, "No Serial Port with id {0}", settings.Port);
            }

            SetPort(port);
        }

        #endregion
    }
}