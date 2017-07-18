using ICD.Connect.Conferencing.Cameras;
using ICD.Connect.Devices;

namespace ICD.Connect.Cameras
{
    public class AbstractCameraDevice<TSettings> : AbstractDevice<TSettings>, ICameraDevice
        where TSettings : ICameraDeviceSettings, new()
    {
        public void Move(eCameraAction action)
        {

        }

        public void Stop()
        {

        }

        protected override bool GetIsOnlineStatus()
        {
            return false;
        }
    }
}