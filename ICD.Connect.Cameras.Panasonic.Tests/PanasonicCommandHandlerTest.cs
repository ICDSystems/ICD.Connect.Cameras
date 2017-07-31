using ICD.Connect.Conferencing.Cameras;
using NUnit.Framework;

namespace ICD.Connect.Cameras.Panasonic.Tests_NetStandard
{
    [TestFixture]
    public sealed class PanasonicCommandHandlerTest
    {
        [TestCase("/cgi-bin/aw_ptz?cmd=#PTS5050&res=1")]
        public void TestStop(string expected)
        {
            PanasonicCommandHandler commandHandler = new PanasonicCommandHandler();
            Assert.AreEqual(expected, commandHandler.Stop());
        }

        [TestCase(eCameraAction.Up, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#PTS7450&res=1")]
        [TestCase(eCameraAction.Down, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#PTS2650&res=1")]
        [TestCase(eCameraAction.Left, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#PTS5026&res=1")]
        [TestCase(eCameraAction.Right, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#PTS5074&res=1")]
        [TestCase(eCameraAction.ZoomIn, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#Z26&res=1")]
        [TestCase(eCameraAction.ZoomOut, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#Z74&res=1")]
        public void TestMove(eCameraAction action, int panSpeed, int tiltSpeed, int zoomSpeed, string expected)
        {
            //ViscaCommandHandler commandHandler = new ViscaCommandHandler();
            PanasonicCommandHandler commandHandler = new PanasonicCommandHandler();
            commandHandler.SetDefaultPanSpeed(panSpeed);
            commandHandler.SetDefaultTiltSpeed(tiltSpeed);
            commandHandler.SetDefaultZoomSpeed(zoomSpeed);
            string result = commandHandler.Move(action);
            Assert.AreEqual(expected, result);
        }
    }
}
