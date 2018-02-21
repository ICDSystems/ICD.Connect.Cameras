using NUnit.Framework;

namespace ICD.Connect.Cameras.Panasonic.Tests
{
	[TestFixture]
	public sealed class PanasonicCommandBuilderTest
	{
		[TestCase("/cgi-bin/aw_ptz?cmd=%23PTS5050&res=1")]
		public void TestStop(string expected)
		{
			Assert.AreEqual(expected, PanasonicCommandBuilder.GetPanTiltCommand(eCameraPanTiltAction.Stop));
		}

		[TestCase(eCameraPanTiltAction.Up, 24, "/cgi-bin/aw_ptz?cmd=%23PTS5074&res=1")]
		[TestCase(eCameraPanTiltAction.Down, 24, "/cgi-bin/aw_ptz?cmd=%23PTS5026&res=1")]
		[TestCase(eCameraPanTiltAction.Left, 24, "/cgi-bin/aw_ptz?cmd=%23PTS2650&res=1")]
		[TestCase(eCameraPanTiltAction.Right, 24, "/cgi-bin/aw_ptz?cmd=%23PTS7450&res=1")]
		public void TestMove(eCameraPanTiltAction action, int panTiltSpeed, string expected)
		{
			string result = PanasonicCommandBuilder.GetPanTiltCommand(action, panTiltSpeed);
			Assert.AreEqual(expected, result);
		}
		
		[TestCase(eCameraZoomAction.ZoomIn, 24, "/cgi-bin/aw_ptz?cmd=%23Z74&res=1")]
		[TestCase(eCameraZoomAction.ZoomOut, 24, "/cgi-bin/aw_ptz?cmd=%23Z26&res=1")]
		public void TestZoom(eCameraZoomAction action, int zoomspeed, string expected)
		{
			string result = PanasonicCommandBuilder.GetZoomCommand(action, zoomspeed);
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void TestPower()
		{
			Assert.AreEqual("/cgi-bin/aw_ptz?cmd=%23O1&res=1", PanasonicCommandBuilder.GetPowerOnCommand());
			Assert.AreEqual("/cgi-bin/aw_ptz?cmd=%23O0&res=1", PanasonicCommandBuilder.GetPowerOffCommand());
			Assert.AreEqual("/cgi-bin/aw_ptz?cmd=%23O&res=1", PanasonicCommandBuilder.GetPowerQueryCommand());
		}


	}
}
