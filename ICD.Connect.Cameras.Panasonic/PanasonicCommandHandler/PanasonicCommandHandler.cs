using ICD.Common.Utils;
using ICD.Connect.Conferencing.Cameras;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICD.Connect.Cameras.Panasonic
{
    public enum ePanasonicResponse
    {
        OK,
        NETWORK_ERROR,
        UNSPECIFIED_ERORR
    }
    public static class PanasonicResponseHandler
    {
        public static ePanasonicResponse HandleResponse(string response)
        {

            return ePanasonicResponse.OK;
        }
    }

    public class PanasonicCommandHandler
    {
        private int m_defaultPanSpeed = 24;
        private int m_defaultTiltSpeed = 24;
        private int m_defaultZoomSpeed = 24;

        private string GetCommandUrl(string command)
        {
            return string.Format("/cgi-bin/aw_ptz?cmd={0}&res=1", command);
        }

        private string GetSpeedBasedOnDirection(eCameraAction action)
        {
            int returnSpeed = 0;
            switch (action)
            {
                case eCameraAction.Down:
                    returnSpeed = 50 - m_defaultTiltSpeed;
                    break;
                case eCameraAction.Up:
                    returnSpeed = 50 + m_defaultTiltSpeed;
                    break;
                case eCameraAction.Left:
                    returnSpeed = 50 - m_defaultPanSpeed;
                    break;
                case eCameraAction.Right:
                    returnSpeed = 50 + m_defaultPanSpeed;
                    break;
                case eCameraAction.ZoomIn:
                    returnSpeed = 50 - m_defaultZoomSpeed;
                    break;
                case eCameraAction.ZoomOut:
                    returnSpeed = 50 + m_defaultZoomSpeed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("action");
            }
            return String.Format("{0:00}", returnSpeed);
        }

        public void SetDefaultPanSpeed(int speed)
        {
            m_defaultPanSpeed = MathUtils.Clamp(speed, 1, 49);
        }

        public void SetDefaultTiltSpeed(int speed)
        {
            m_defaultTiltSpeed = MathUtils.Clamp(speed, 1, 49);
        }

        public void SetDefaultZoomSpeed(int speed)
        {
            m_defaultZoomSpeed = MathUtils.Clamp(speed, 1, 49);
        }


        public string Stop()
        {
            return GetCommandUrl("#PTS5050");
        }

        public string Move(eCameraAction action)
        {
            var speed = GetSpeedBasedOnDirection(action);

            switch (action)
            {
                case eCameraAction.Down:
                case eCameraAction.Up:
                    return GetCommandUrl(String.Format("#PTS50{0}", speed));

                case eCameraAction.Left:
                case eCameraAction.Right:
                    return GetCommandUrl(String.Format("#PTS{0}50", speed));

                case eCameraAction.ZoomIn:
                case eCameraAction.ZoomOut:
                    return GetCommandUrl(String.Format("#Z{0}", speed));
                default:
                    throw new ArgumentOutOfRangeException("action");
            }
        }
    }
}
