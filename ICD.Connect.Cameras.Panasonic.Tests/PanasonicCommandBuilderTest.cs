//TODO: FIX ME
//using NUnit.Framework;

//namespace ICD.Connect.Cameras.Panasonic.Tests
//{
//    [TestFixture]
//    public sealed class PanasonicCommandBuilderTest
//    {
//        [TestCase("/cgi-bin/aw_ptz?cmd=#PTS5050&res=1")]
//        public void TestStop(string expected)
//        {

//			PanasonicCommandBuilder commandHandler = new PanasonicCommandHandler();
//			Assert.AreEqual(expected, commandHandler.Stop());
//		}

//        [TestCase(eCameraPanTiltAction.Up, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#PTS7450&res=1")]
//        [TestCase(eCameraPanTiltAction.Down, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#PTS2650&res=1")]
//        [TestCase(eCameraPanTiltAction.Left, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#PTS5026&res=1")]
//        [TestCase(eCameraPanTiltAction.Right, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#PTS5074&res=1")]
//        [TestCase(eCameraAction.ZoomIn, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#Z26&res=1")]
//        [TestCase(eCameraAction.ZoomOut, 24, 24, 24, "/cgi-bin/aw_ptz?cmd=#Z74&res=1")]
//        public void TestMove(eCameraAction action, int panSpeed, int tiltSpeed, int zoomSpeed, string expected)
//        {
//            //ViscaCommandHandler commandHandler = new ViscaCommandHandler();
//            PanasonicCommandHandler commandHandler = new PanasonicCommandHandler();
//            commandHandler.SetDefaultPanSpeed(panSpeed);
//            commandHandler.SetDefaultTiltSpeed(tiltSpeed);
//            commandHandler.SetDefaultZoomSpeed(zoomSpeed);
//            string result = commandHandler.Move(action);
//            Assert.AreEqual(expected, result);
//        }
//    }
//}
