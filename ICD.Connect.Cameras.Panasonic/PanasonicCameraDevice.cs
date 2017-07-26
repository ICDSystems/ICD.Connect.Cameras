using ICD.Connect.Conferencing.Cameras;

namespace ICD.Connect.Cameras.Panasonic
{
    public sealed class PanasonicCameraDevice : AbstractCameraDevice<PanasonicCameraDeviceSettings>
    {
        public override void Move(eCameraAction action)
        {
        }

        public override void Stop()
        {
        }

        /// <summary>
        /// Gets the current online status of the device.
        /// </summary>
        /// <returns></returns>
        protected override bool GetIsOnlineStatus()
        {
            return false;
            //return m_Port != null && m_Port.IsOnline;
        }
    }
}