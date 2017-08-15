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

        [Test]
        public void StopZoom()
        {
            string result = PanasonicCommandBuilder.StopZoom();
            Assert.AreEqual("cgi-bin/aw_ptz?cmd=%23Z50&res=1", result);
        }

        [TestCase(eCameraAction.Up, "cgi-bin/aw_ptz?cmd=%23PTS5074&res=1")]
        [TestCase(eCameraAction.Down, "cgi-bin/aw_ptz?cmd=%23PTS5026&res=1")]
        [TestCase(eCameraAction.Left, "cgi-bin/aw_ptz?cmd=%23PTS2650&res=1")]
        [TestCase(eCameraAction.Right, "cgi-bin/aw_ptz?cmd=%23PTS7450&res=1")]
        [TestCase(eCameraAction.ZoomIn, "cgi-bin/aw_ptz?cmd=%23Z74&res=1")]
        [TestCase(eCameraAction.ZoomOut, "cgi-bin/aw_ptz?cmd=%23Z26&res=1")]
        public void Move(eCameraAction action, string expected)
        {
            string result = PanasonicCommandBuilder.Move(action);
            Assert.AreEqual(expected, result);
        }
    }
}
