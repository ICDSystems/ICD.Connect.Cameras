using ICD.Common.EventArguments;
using ICD.Common.Services.Logging;
using ICD.Connect.Conferencing.Cameras;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Settings.Core;
using ICD.Common.Utils;
using System;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Common.Properties;
using ICD.Connect.Protocol.SerialQueues;
using ICD.Connect.Protocol.EventArguments;
using ICD.Connect.Protocol.Data;
using ICD.Connect.Protocol.SerialBuffers;
using System.Collections.Generic;
//using ICD.Connect.Cameras.Visca.CommandBuilder;
//using System.Threading;

namespace ICD.Connect.Cameras.Visca
{

    public sealed class ViscaCameraDevice : AbstractCameraDevice<ViscaCameraDeviceSettings>
    {
        private ISerialBuffer SerialBuffer { get; set; }
        private ISerialQueue SerialQueue { get; set; }
        private ViscaCommandHandler CommandHandler { get; set; }

        private readonly Dictionary<string, int> m_RetryCounts = new Dictionary<string, int>();
        private readonly SafeCriticalSection m_RetryLock = new SafeCriticalSection();

        private const int MAX_RETRY_ATTEMPTS = 20;

        private const char DELIMITER = '\xFF';

        #region Methods
        //Camera Methods

        public ViscaCameraDevice()
        {
            Controls.Add(new GenericCameraRouteSourceControl<ViscaCameraDevice>(this, 0));
        }

        public override void Move(eCameraAction action)
        {
             SendCommand(CommandHandler.Move(1, action));
        }

        public override void Stop()
        {
            SendCommand(CommandHandler.Stop(1));
        }

        /// <summary>
        /// Release resources.
        /// </summary>
        protected override void DisposeFinal(bool disposing)
        {
            SetPort(null);

            base.DisposeFinal(disposing);
        }

        //Class Settings

        /// <summary>
        /// Sets the wrapped port for communication with the hardware.
        /// </summary>
        /// <param name="port"></param>
        public void SetPort(ISerialPort port)
        {
            if (port is IComPort)
                ConfigureComPort(port as IComPort);

            ISerialBuffer buffer = new DelimiterSerialBuffer(DELIMITER);
            SerialQueue queue = new SerialQueue();
            queue.SetPort(port);
            queue.SetBuffer(buffer);
            queue.Timeout = 3 * 1000;

            SetSerialQueue(queue);
        }

        [PublicAPI]
        public static void ConfigureComPort(IComPort port)
        {
            port.SetComPortSpec(
                eComBaudRates.ComspecBaudRate9600,
                eComDataBits.ComspecDataBits8,
                eComParityType.ComspecParityNone,
                eComStopBits.ComspecStopBits1,
                eComProtocolType.ComspecProtocolRS232,
                eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
                false);
        }

        /// <summary>
        /// Gets the current online status of the device.
        /// </summary>
        /// <returns></returns>
        protected override bool GetIsOnlineStatus()
        {
            return SerialQueue != null && SerialQueue.Port != null && SerialQueue.Port.IsOnline;
        }

        #endregion

        #region Port Callbacks

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

        #region SerialQueue Callbacks

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

            IcdConsole.PrintLine(String.Format("Serial Data {0} - Serial Response {1}", StringUtils.ToHexLiteral(args.Data.Serialize()), StringUtils.ToHexLiteral(args.Response)));

            if (ViscaResponseHandler.HandleResponse(args.Response) == eViscaResponse.OK)
            {
                ParseQuery(args.Response);
            }
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
            {
                SerialQueue.EnqueuePriority(new SerialData(command));
            }
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
                {
                    m_RetryCounts[command]++;
                }
                else
                {
                    m_RetryCounts.Add(command, 1);
                }

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

        private void SerialQueueOnIsOnlineStateChanged(object sender, BoolEventArgs args)
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
            {
                settings.Port = SerialQueue.Port.Id;
            }
            else
                settings.Port = null;
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
            {
                port = factory.GetPortById((int)settings.Port) as ISerialPort;
            }

            if (port == null)
            {
                Logger.AddEntry(eSeverity.Error, String.Format("No serial port with Id {0}", settings.Port));
            }

            SetPort(port);
            if (port.IsOnline)
            {
                CommandHandler = new ViscaCommandHandler();
                SendCommand(CommandHandler.SetAddress());
                SendCommand(CommandHandler.Clear(1));

            }
        }

        #endregion

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
        /// <summary>
        /// Queues the command to be sent to the device.
        /// Replaces an existing command if it matches the comparer.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="comparer"></param>
        public void SendCommand(string data, Func<string, string, bool> comparer)
        {
            //SerialQueue.Enqueue(command, comparer);
            SendCommand(new SerialData(data), (a, b) => comparer(a.Serialize(), b.Serialize()));
        }

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

        /// <summary>
        /// Logs to logging core.
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        private void Log(eSeverity severity, string message, params object[] args)
        {
            message = string.Format(message, args);
            message = string.Format("{0} - {1}", this, message);

            Logger.AddEntry(severity, message);
        }
    }
}