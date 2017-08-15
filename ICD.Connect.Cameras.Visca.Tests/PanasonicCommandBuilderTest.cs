using ICD.Connect.Conferencing.Cameras;
using NUnit.Framework;

namespace ICD.Connect.Cameras.Panasonic.Tests_NetStandard
{
    [TestFixture]
    public sealed class PanasonicCommandBuilderTest
    {

        [Test]
        public void Stop()
        {
            string result = PanasonicCommandBuilder.Stop();
            Assert.AreEqual("cgi-bin/aw_ptz?cmd=%23PTS5050&res=1", result);
        }

        /*
        /// <summary>
        /// Moves the camera or zooms in, based on eCameraAction.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="action"></param>
        /// <param name="panSpeed"></param>
        /// <param name="tiltSpeed"></param>
        /// <param name="zoomSpeed"></param>
        /// <returns></returns>
        [TestCase(eCameraAction.Up, 48, 48, 48, "/cgi-bin/aw_ptz?cmd=#PTS5098&res=1")]
        [TestCase(eCameraAction.Down, 48, 48, 48, "/cgi-bin/aw_ptz?cmd=#PTS5002&res=1")]
        [TestCase(eCameraAction.Left, 48, 48, 48, "/cgi-bin/aw_ptz?cmd=#PTS0250&res=1")]
        [TestCase(eCameraAction.Right, 48, 48, 48, "/cgi-bin/aw_ptz?cmd=#PTS9850&res=1")]
        [TestCase(eCameraAction.ZoomIn, 48, 48, 48, "/cgi-bin/aw_ptz?cmd=#Z02&res=1")]
        [TestCase(eCameraAction.ZoomOut, 48, 48, 48, "/cgi-bin/aw_ptz?cmd=#Z98&res=1")]
        public void Move(eCameraAction action, int panSpeed, int tiltSpeed, int zoomSpeed, string expected)
        {
            commandHandler.SetDefaultPanSpeed(panSpeed);
            commandHandler.SetDefaultTiltSpeed(tiltSpeed);
            commandHandler.SetDefaultZoomSpeed(zoomSpeed);
            string result = commandHandler.Move(action);
            Assert.AreEqual(expected, result);
        }
        */
    }
}
