using ICD.Common.Utils;
using NUnit.Framework;

namespace ICD.Connect.Cameras.Visca.Tests
{
	[TestFixture]
	public sealed class ViscaCommandBuilderTest
	{
		[Test]
		public static void GetPanTiltCommandTest()
		{
			string expectedTiltUpCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x01, 0x01, 0x03, 0x01, 0xFF });
			string expectedtiltDownCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x01, 0x01, 0x03, 0x02, 0xFF });
			string expectedpanLeftCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x01, 0x01, 0x01, 0x03, 0xFF });
			string expectedpanRightCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x01, 0x01, 0x02, 0x03, 0xFF });
			string tiltUpCommand = ViscaCommandBuilder.GetPanTiltCommand(0, eCameraPanTiltAction.Up, 1, 1);
			string tiltDownCommand = ViscaCommandBuilder.GetPanTiltCommand(0, eCameraPanTiltAction.Down, 1, 1);
			string panLeftCommand = ViscaCommandBuilder.GetPanTiltCommand(0, eCameraPanTiltAction.Left, 1, 1);
			string panRightCommand = ViscaCommandBuilder.GetPanTiltCommand(0, eCameraPanTiltAction.Right, 1, 1);
			Assert.AreEqual(expectedTiltUpCommand, tiltUpCommand);
			Assert.AreEqual(expectedtiltDownCommand, tiltDownCommand);
			Assert.AreEqual(expectedpanLeftCommand, panLeftCommand);
			Assert.AreEqual(expectedpanRightCommand, panRightCommand);
		}

		[Test]
		public static void GetZoomCommandTest()
		{
			string expectedZoomInCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x04, 0x07, 0x21, 0xFF });
			string expectedZoomOutCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x04, 0x07, 0x31, 0xFF });
			string zoomInCommand = ViscaCommandBuilder.GetZoomCommand(0, eCameraZoomAction.ZoomIn, 1);
			string zoomOutCommand = ViscaCommandBuilder.GetZoomCommand(0, eCameraZoomAction.ZoomOut, 1);
			Assert.AreEqual(expectedZoomInCommand, zoomInCommand);
			Assert.AreEqual(expectedZoomOutCommand, zoomOutCommand);
		}

		[Test]
		public static void GetSetAddressCommandTest()
		{
			string expectedGetAddressCommand = StringUtils.ToString(new byte[] { 0x88, 0x30, 0x01, 0xFF });
			string getAddressCommand = ViscaCommandBuilder.GetSetAddressCommand();
			Assert.AreEqual(expectedGetAddressCommand, getAddressCommand);
		}

		[Test]
		public static void GetClearCommandTest()
		{
			string expectedClearCommand = StringUtils.ToString(new byte[] { 0x88, 0x01, 0x00, 0x01, 0xFF });
			string clearCommand = ViscaCommandBuilder.GetClearCommand();
			Assert.AreEqual(expectedClearCommand, clearCommand);
		}


		[Test]
		public static void GetPowerOnCommandTest()
		{
			string expectedPowerOnCommand = StringUtils.ToString(new byte[] {0x81, 0x01, 0x04, 0x00, 0x02, 0xFF});
			string powerOnCommand = ViscaCommandBuilder.GetPowerOnCommand(1);
			Assert.AreEqual(expectedPowerOnCommand, powerOnCommand);
		}

		[Test]
		public static void GetPowerOffCommandTest()
		{
			string expectedPowerOffCommand = StringUtils.ToString(new byte[] { 0x81, 0x01, 0x04, 0x00, 0x03, 0xFF });
			string powerOffCommand = ViscaCommandBuilder.GetPowerOffCommand(1);
			Assert.AreEqual(expectedPowerOffCommand, powerOffCommand);
		}
	}
}
